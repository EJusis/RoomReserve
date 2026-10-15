namespace RoomReserve.Application.Reservations.Booking.Rules;

public class ValidTimeRangeRule(TimeProvider time) : SyncBookingRule
{
    protected override BookingError? Check(BookingRequest r)
    {
        if (r.End <= r.Start)
            return new BookingError(BookingErrorCodes.InvalidTimeRange,
                "End time must be after start time.");

        if (r.Start <= time.GetUtcNow())
            return new BookingError(BookingErrorCodes.StartInPast,
                "Start time must be in the future.");

        return null;
    }
}