using RoomReserve.Domain.Entities;

namespace RoomReserve.Application.Reservations.Booking;

/// <summary>A reservation candidate to validate (new or edited).</summary>
public record BookingRequest(
    Room Room,
    int OrganizerId,
    DateTimeOffset Start,
    DateTimeOffset End,
    int AttendeeCount,
    int? ExcludeReservationId = null);