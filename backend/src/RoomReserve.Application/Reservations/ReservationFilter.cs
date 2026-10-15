using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Reservations;

public record ReservationFilter
{
    public int? RoomId { get; init; }
    public int? OrganizerId { get; init; }
    public ReservationStatus? Status { get; init; }

    /// <summary>Reservations that end after this moment.</summary>
    public DateTimeOffset? From { get; init; }

    /// <summary>Reservations that start before this moment.</summary>
    public DateTimeOffset? To { get; init; }
}