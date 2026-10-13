using RoomReserve.Domain.Enums;

namespace RoomReserve.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;

    public required string Title { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int AttendeeCount { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Booked;
    public DateTimeOffset? EndedEarlyAt { get; set; }

    /// <summary>
    /// A booked meeting whose end time has passed counts as Completed,
    /// without anyone having to update it.
    /// </summary>
    public ReservationStatus GetStatus(DateTimeOffset now) =>
        Status == ReservationStatus.Booked && End <= now
            ? ReservationStatus.Completed
            : Status;
}