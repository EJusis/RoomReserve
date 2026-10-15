using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Domain.Entities;

namespace RoomReserve.Tests.Booking;

internal static class TestData
{
    // A fixed "now" makes every test deterministic
    public static readonly DateTimeOffset Now = new(2026, 1, 15, 9, 0, 0, TimeSpan.Zero);

    public static FakeTimeProvider Clock() => new(Now);

    public static IOptions<BookingOptions> Settings(int maxDays = 7, int horizonDays = 90) =>
        Options.Create(new BookingOptions { MaxDurationDays = maxDays, BookingHorizonDays = horizonDays });

    public static Room NewRoom(int id = 1, int capacity = 10, bool isActive = true) => new()
    {
        Id = id, Name = "Test room", Location = "Test wing", Floor = 1,
        Capacity = capacity, IsActive = isActive
    };

    public static BookingRequest NewRequest(
        DateTimeOffset? start = null, TimeSpan? duration = null, int attendees = 5,
        Room? room = null, int organizerId = 1, int? excludeId = null)
    {
        var s = start ?? Now.AddDays(1);
        return new BookingRequest(room ?? NewRoom(), organizerId, s,
            s + (duration ?? TimeSpan.FromHours(1)), attendees, excludeId);
    }
}