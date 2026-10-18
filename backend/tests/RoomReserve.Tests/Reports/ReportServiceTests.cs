using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomReserve.Application.Reports;
using RoomReserve.Domain.Entities;
using RoomReserve.Infrastructure.Persistence;
using RoomReserve.Tests.Booking;

namespace RoomReserve.Tests.Reports;

public class ReportServiceTests
{
    private readonly AppDbContext _db = new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _db.Rooms.Add(TestData.NewRoom());
        _db.SaveChanges();
        _service = new ReportService(_db, TestData.Clock(), Options.Create(new ReportOptions()));
    }

    private void AddMeeting(DateTimeOffset start, double hours)
    {
        _db.Reservations.Add(new Reservation
        {
            RoomId = 1, OrganizerId = 1, Title = "Meeting", AttendeeCount = 2,
            Start = start, End = start.AddHours(hours)
        });
        _db.SaveChanges();
    }

    [Fact]
    public async Task Future_bookings_are_not_counted_as_unused()
    {
        AddMeeting(TestData.Now.AddDays(-2), hours: 2); // happened
        AddMeeting(TestData.Now.AddDays(1), hours: 2);  // hasn't happened yet

        var result = await _service.GetRoomUsageAsync(null, TestData.Now.AddDays(7));

        var room = Assert.Single(result.Value!.Rooms);
        Assert.Equal(1, room.MeetingCount);
        Assert.Equal(1.0, room.Utilization);
    }

    [Fact]
    public async Task Period_ending_before_it_starts_is_rejected()
    {
        var result = await _service.GetRoomUsageAsync(TestData.Now, TestData.Now.AddDays(-1));

        Assert.Equal(ReportErrors.InvalidPeriod.Code, Assert.Single(result.Errors).Code);
    }
}