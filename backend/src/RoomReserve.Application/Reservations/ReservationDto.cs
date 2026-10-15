using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Reservations;

public record ReservationDto(
    int Id,
    int RoomId,
    string RoomName,
    int OrganizerId,
    string OrganizerName,
    string Title,
    DateTimeOffset Start,
    DateTimeOffset End,
    DateTimeOffset? EndedEarlyAt,
    int AttendeeCount,
    ReservationStatus Status);