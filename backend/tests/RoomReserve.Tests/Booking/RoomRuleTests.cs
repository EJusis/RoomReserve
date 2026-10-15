using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;

namespace RoomReserve.Tests.Booking;

// Covers R5 and R7

public class RoomRuleTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)] // exactly at capacity
    public async Task Attendees_within_capacity_are_allowed(int attendees)
    {
        var request = TestData.NewRequest(attendees: attendees, room: TestData.NewRoom(capacity: 10));
        Assert.Null(await new CapacityRule().CheckAsync(request, default));
    }

    [Fact]
    public async Task Attendees_over_capacity_are_rejected()
    {
        var request = TestData.NewRequest(attendees: 11, room: TestData.NewRoom(capacity: 10));
        var error = await new CapacityRule().CheckAsync(request, default);
        Assert.Equal(BookingErrorCodes.CapacityExceeded, error?.Code);
    }

    [Fact]
    public async Task Inactive_room_is_rejected()
    {
        var request = TestData.NewRequest(room: TestData.NewRoom(isActive: false));
        var error = await new ActiveRoomRule().CheckAsync(request, default);
        Assert.Equal(BookingErrorCodes.RoomInactive, error?.Code);
    }
}