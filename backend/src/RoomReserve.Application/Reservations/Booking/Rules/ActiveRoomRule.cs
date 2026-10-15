namespace RoomReserve.Application.Reservations.Booking.Rules;

public class ActiveRoomRule : SyncBookingRule
{
    protected override BookingError? Check(BookingRequest r) =>
        r.Room.IsActive
            ? null
            : new BookingError(BookingErrorCodes.RoomInactive,
                $"Room '{r.Room.Name}' is inactive and cannot be booked.");
}