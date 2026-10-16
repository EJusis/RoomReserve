using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using RoomReserve.Application.Reservations;
using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;
using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;
using RoomReserve.Domain.Errors;
using RoomReserve.Infrastructure.Persistence;
using RoomReserve.Tests.Booking;

namespace RoomReserve.Tests.Reservations;

public class ReservationServiceTests
{
    private readonly AppDbContext _db = new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private readonly FakeTimeProvider _clock = TestData.Clock();
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _db.Rooms.AddRange(
            new Room { Id = 1, Name = "Open room", Location = "A", Floor = 1, Capacity = 10 },
            new Room { Id = 2, Name = "Closed room", Location = "B", Floor = 1, Capacity = 10, IsActive = false });
        _db.Users.Add(new User { Id = 1, FirstName = "Test", LastName = "User" });
        _db.SaveChanges();

        var options = TestData.Settings();
        var validator = new BookingValidator(
        [
            new ValidTimeRangeRule(_clock),
            new MaxDurationRule(options),
            new BookingHorizonRule(options, _clock),
            new ActiveRoomRule(),
            new CapacityRule(),
            new RoomAvailabilityRule(_db),
            new OrganizerAvailabilityRule(_db)
        ]);

        _service = new ReservationService(_db, validator, _clock);
    }

    private static CreateReservationRequest NewCreate(int roomId = 1, int startsInHours = 24) => new()
    {
        RoomId = roomId,
        OrganizerId = 1,
        Title = "Planning",
        Start = TestData.Now.AddHours(startsInHours),
        End = TestData.Now.AddHours(startsInHours + 1),
        AttendeeCount = 4
    };

    private async Task<ReservationDto> CreateAsync(int startsInHours = 24) =>
        (await _service.CreateAsync(NewCreate(startsInHours: startsInHours))).Value!;

    [Fact]
    public async Task Valid_reservation_is_created_as_booked()
    {
        var result = await _service.CreateAsync(NewCreate());

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Booked, result.Value!.Status);
        Assert.Equal(1, await _db.Reservations.CountAsync());
    }

    [Fact]
    public async Task Rule_violation_is_returned_and_nothing_is_saved()
    {
        var result = await _service.CreateAsync(NewCreate(roomId: 2));

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Code == BookingErrorCodes.RoomInactive);
        Assert.Equal(0, await _db.Reservations.CountAsync());
    }

    [Fact]
    public async Task Unknown_room_returns_not_found()
    {
        var result = await _service.CreateAsync(NewCreate(roomId: 99));

        Assert.Equal(ErrorType.NotFound, Assert.Single(result.Errors).Type);
    }

    [Fact]
    public async Task Future_reservation_can_be_cancelled()
    {
        var created = await CreateAsync();

        var result = await _service.CancelAsync(created.Id);

        Assert.Equal(ReservationStatus.Cancelled, result.Value!.Status);
    }

    [Fact]
    public async Task Started_meeting_cannot_be_cancelled()
    {
        var created = await CreateAsync(startsInHours: 1);
        _clock.Advance(TimeSpan.FromMinutes(90)); // meeting is now in progress

        var result = await _service.CancelAsync(created.Id);

        Assert.Equal(ReservationErrors.AlreadyStarted.Code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public async Task Ending_early_records_actual_end_time()
    {
        var created = await CreateAsync(startsInHours: 1);
        _clock.Advance(TimeSpan.FromMinutes(90));

        var result = await _service.EndEarlyAsync(created.Id);

        Assert.Equal(ReservationStatus.Completed, result.Value!.Status);
        Assert.Equal(_clock.GetUtcNow(), result.Value.EndedEarlyAt);
    }

    [Fact]
    public async Task Meeting_cannot_be_ended_before_it_starts()
    {
        var created = await CreateAsync();

        var result = await _service.EndEarlyAsync(created.Id);

        Assert.Equal(ReservationErrors.NotStarted.Code, Assert.Single(result.Errors).Code);
    }

    [Fact]
    public async Task Booked_meeting_in_the_past_is_listed_as_completed()
    {
        await CreateAsync(startsInHours: 1);
        _clock.Advance(TimeSpan.FromHours(3)); // meeting is over

        var completed = await _service.GetReservationsAsync(
            new ReservationFilter { Status = ReservationStatus.Completed });

        Assert.Single(completed);
    }
}