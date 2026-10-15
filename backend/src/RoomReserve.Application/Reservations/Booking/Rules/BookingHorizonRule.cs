using Microsoft.Extensions.Options;

namespace RoomReserve.Application.Reservations.Booking.Rules;

public class BookingHorizonRule(IOptions<BookingOptions> options, TimeProvider time) : SyncBookingRule
{
    protected override BookingError? Check(BookingRequest r)
    {
        var horizonDays = options.Value.BookingHorizonDays;

        return r.Start > time.GetUtcNow().AddDays(horizonDays)
            ? new BookingError(BookingErrorCodes.BeyondBookingHorizon,
                $"A reservation cannot start more than {horizonDays} days in the future.")
            : null;
    }
}