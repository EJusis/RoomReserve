using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;

namespace RoomReserve.Application.Reservations.Booking.Rules;

public class RoomAvailabilityRule(IAppDbContext db) : IBookingRule
{
    public async Task<BookingError?> CheckAsync(BookingRequest r, CancellationToken ct)
    {
        var conflict = await db.Reservations
            .AsNoTracking()
            .Where(x => x.RoomId == r.Room.Id)
            .OverlappingBooked(r.Start, r.End, r.ExcludeReservationId)
            .OrderBy(x => x.Start)
            .FirstOrDefaultAsync(ct);

        return conflict is null
            ? null
            : new BookingError(BookingErrorCodes.RoomOverlap,
                $"Room '{r.Room.Name}' is already booked for '{conflict.Title}' at that time.");
    }
}