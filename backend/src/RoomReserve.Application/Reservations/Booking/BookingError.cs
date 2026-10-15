namespace RoomReserve.Application.Reservations.Booking;

public record BookingError(string Code, string Message);

public static class BookingErrorCodes
{
    public const string InvalidTimeRange     = "INVALID_TIME_RANGE";
    public const string StartInPast          = "START_IN_PAST";
    public const string DurationTooLong      = "DURATION_TOO_LONG";
    public const string BeyondBookingHorizon = "BEYOND_BOOKING_HORIZON";
    public const string CapacityExceeded     = "CAPACITY_EXCEEDED";
    public const string RoomInactive         = "ROOM_INACTIVE";
    public const string RoomOverlap          = "ROOM_OVERLAP";
    public const string OrganizerOverlap     = "ORGANIZER_OVERLAP";
}