using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;

namespace RoomReserve.Application.Reservations.Booking.Rules;

public class OrganizerAvailabilityRule(IAppDbContext db) : IBookingRule
{
    public async Task<BookingError?> CheckAsync(BookingRequest r, CancellationToken ct)
    {
        var conflict = await db.Reservations
            .AsNoTracking()
            .Where(x => x.OrganizerId == r.OrganizerId)
            .OverlappingBooked(r.Start, r.End, r.ExcludeReservationId)
            .OrderBy(x => x.Start)
            .FirstOrDefaultAsync(ct);

        return conflict is null
            ? null
            : new BookingError(BookingErrorCodes.OrganizerOverlap,
                $"The organizer already has an overlapping meeting: '{conflict.Title}'.");
    }
}