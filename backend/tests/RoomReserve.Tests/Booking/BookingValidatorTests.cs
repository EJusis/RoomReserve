using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;

namespace RoomReserve.Tests.Booking;

public class BookingValidatorTests
{
    private readonly BookingValidator _validator = new([new CapacityRule(), new ActiveRoomRule()]);

    [Fact]
    public async Task Valid_request_has_no_errors()
    {
        var errors = await _validator.ValidateAsync(TestData.NewRequest());
        Assert.Empty(errors);
    }

    [Fact]
    public async Task All_broken_rules_are_reported_together()
    {
        var room = TestData.NewRoom(capacity: 2, isActive: false);
        var errors = await _validator.ValidateAsync(TestData.NewRequest(attendees: 5, room: room));

        Assert.Equal(
            [BookingErrorCodes.RoomInactive, BookingErrorCodes.CapacityExceeded],
            errors.Select(e => e.Code).OrderByDescending(c => c));
    }
}