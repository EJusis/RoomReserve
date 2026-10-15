using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Reservations.Booking;

public static class ReservationQueries
{
    /// <summary>
    /// Booked reservations intersecting [start, end).
    /// Back-to-back ranges (10–11 and 11–12) do not overlap.
    /// </summary>
    public static IQueryable<Reservation> OverlappingBooked(
        this IQueryable<Reservation> query,
        DateTimeOffset start,
        DateTimeOffset end,
        int? excludeReservationId = null) =>
        query.Where(r => r.Status == ReservationStatus.Booked
                      && r.Id != excludeReservationId
                      && r.Start < end
                      && start < r.End);
}