namespace RoomReserve.Application.Reservations.Booking.Rules;

public class CapacityRule : SyncBookingRule
{
    protected override BookingError? Check(BookingRequest r) =>
        r.AttendeeCount > r.Room.Capacity
            ? new BookingError(BookingErrorCodes.CapacityExceeded,
                $"Room '{r.Room.Name}' fits {r.Room.Capacity} people, but {r.AttendeeCount} attendees were requested.")
            : null;
}