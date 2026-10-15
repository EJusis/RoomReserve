using RoomReserve.Domain.Entities;

namespace RoomReserve.Application.Reservations;

public static class ReservationMappings
{
    public static ReservationDto ToDto(this Reservation r, DateTimeOffset now) => new(
        r.Id,
        r.RoomId,
        r.Room.Name,
        r.OrganizerId,
        $"{r.Organizer.FirstName} {r.Organizer.LastName}",
        r.Title,
        r.Start,
        r.End,
        r.EndedEarlyAt,
        r.AttendeeCount,
        r.GetStatus(now));
}