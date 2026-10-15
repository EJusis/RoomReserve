namespace RoomReserve.Application.Reservations.Booking;

public interface IBookingRule
{
    Task<BookingError?> CheckAsync(BookingRequest request, CancellationToken ct);
}

/// <summary>Base for rules that don't need the database.</summary>
public abstract class SyncBookingRule : IBookingRule
{
    public Task<BookingError?> CheckAsync(BookingRequest request, CancellationToken ct)
        => Task.FromResult(Check(request));

    protected abstract BookingError? Check(BookingRequest request);
}