using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;
using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;
using RoomReserve.Infrastructure.Persistence;

namespace RoomReserve.Tests.Booking;

// Covers R2 and R3

public class OverlapRuleTests
{
    // Each test class instance gets its own empty database
    private readonly AppDbContext _db = new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static readonly DateTimeOffset TenAm = new(2026, 1, 16, 10, 0, 0, TimeSpan.Zero);

    private async Task<Reservation> ExistingAsync(
        int roomId, int organizerId, DateTimeOffset start, double hours,
        ReservationStatus status = ReservationStatus.Booked)
    {
        var reservation = new Reservation
        {
            RoomId = roomId, OrganizerId = organizerId, Title = "Existing meeting",
            Start = start, End = start.AddHours(hours), AttendeeCount = 2, Status = status
        };
        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();
        return reservation;
    }

    private Task<BookingError?> CheckRoomAsync(BookingRequest r) =>
        new RoomAvailabilityRule(_db).CheckAsync(r, default);

    [Fact]
    public async Task Overlapping_booking_in_same_room_is_rejected()
    {
        await ExistingAsync(roomId: 1, organizerId: 2, TenAm, hours: 1);

        var error = await CheckRoomAsync(TestData.NewRequest(start: TenAm.AddMinutes(30)));

        Assert.Equal(BookingErrorCodes.RoomOverlap, error?.Code);
    }

    [Fact]
    public async Task Back_to_back_booking_is_allowed()
    {
        await ExistingAsync(roomId: 1, organizerId: 2, TenAm, hours: 1);

        var error = await CheckRoomAsync(TestData.NewRequest(start: TenAm.AddHours(1)));

        Assert.Null(error);
    }

    [Theory]
    [InlineData(ReservationStatus.Cancelled)]
    [InlineData(ReservationStatus.Completed)] // ended early
    public async Task Cancelled_or_ended_reservation_does_not_block_room(ReservationStatus status)
    {
        await ExistingAsync(roomId: 1, organizerId: 2, TenAm, hours: 1, status);

        var error = await CheckRoomAsync(TestData.NewRequest(start: TenAm));

        Assert.Null(error);
    }

    [Fact]
    public async Task Edited_reservation_does_not_conflict_with_itself()
    {
        var existing = await ExistingAsync(roomId: 1, organizerId: 1, TenAm, hours: 1);

        var error = await CheckRoomAsync(TestData.NewRequest(start: TenAm, excludeId: existing.Id));

        Assert.Null(error);
    }

    [Fact]
    public async Task Organizer_cannot_overlap_in_a_different_room()
    {
        await ExistingAsync(roomId: 2, organizerId: 1, TenAm, hours: 1);

        var request = TestData.NewRequest(start: TenAm.AddMinutes(30), organizerId: 1);
        var error = await new OrganizerAvailabilityRule(_db).CheckAsync(request, default);

        Assert.Equal(BookingErrorCodes.OrganizerOverlap, error?.Code);
    }
}