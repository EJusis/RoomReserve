using System.ComponentModel.DataAnnotations;

namespace RoomReserve.Application.Reservations;

public record CreateReservationRequest
{
    [Range(1, int.MaxValue)]
    public int RoomId { get; init; }

    [Range(1, int.MaxValue)]
    public int OrganizerId { get; init; }

    [Required, StringLength(200)]
    public string Title { get; init; } = "";

    public DateTimeOffset Start { get; init; }
    public DateTimeOffset End { get; init; }

    [Range(1, 1000)]
    public int AttendeeCount { get; init; }
}