using Microsoft.Extensions.Options;

namespace RoomReserve.Application.Reservations.Booking.Rules;

public class MaxDurationRule(IOptions<BookingOptions> options) : SyncBookingRule
{
    protected override BookingError? Check(BookingRequest r)
    {
        var maxDays = options.Value.MaxDurationDays;

        return r.End - r.Start > TimeSpan.FromDays(maxDays)
            ? new BookingError(BookingErrorCodes.DurationTooLong,
                $"A reservation cannot last longer than {maxDays} days.")
            : null;
    }
}