namespace RoomReserve.Application.Reservations.Booking;

public class BookingValidator(IEnumerable<IBookingRule> rules)
{
    public async Task<IReadOnlyList<BookingError>> ValidateAsync(
        BookingRequest request, CancellationToken ct = default)
    {
        var errors = new List<BookingError>();

        foreach (var rule in rules)
        {
            if (await rule.CheckAsync(request, ct) is { } error)
                errors.Add(error);
        }

        return errors;
    }
}