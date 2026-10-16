using RoomReserve.Domain.Enums;
using RoomReserve.Domain.Errors;

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

                /// <summary>Cancel a reservation that has not started yet.</summary>
        public Error? Cancel(DateTimeOffset now)
    {
        var error = CancelError(now);
        if (error is null)
            Status = ReservationStatus.Cancelled;
        return error;
    }

    public Error? EndEarly(DateTimeOffset now)
    {
        var error = EndEarlyError(now);
        if (error is null)
        {
            Status = ReservationStatus.Completed;
            EndedEarlyAt = now;
        }
        return error;
    }

    public bool CanCancel(DateTimeOffset now) => CancelError(now) is null;
    public bool CanEndEarly(DateTimeOffset now) => EndEarlyError(now) is null;

    // Cancel only before the meeting starts
    private Error? CancelError(DateTimeOffset now) => GetStatus(now) switch
    {
        ReservationStatus.Cancelled => ReservationErrors.AlreadyCancelled,
        ReservationStatus.Completed => ReservationErrors.AlreadyCompleted,
        _ when Start <= now         => ReservationErrors.AlreadyStarted,
        _                           => null
    };

    // End early only while the meeting is in progress
    private Error? EndEarlyError(DateTimeOffset now) => GetStatus(now) switch
    {
        ReservationStatus.Cancelled => ReservationErrors.AlreadyCancelled,
        ReservationStatus.Completed => ReservationErrors.AlreadyCompleted,
        _ when now < Start          => ReservationErrors.NotStarted,
        _                           => null
    };

}

