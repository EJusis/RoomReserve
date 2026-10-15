namespace RoomReserve.Application.Reservations.Booking;

public class BookingOptions
{
    public const string SectionName = "Booking";

    public int MaxDurationDays { get; set; } = 7;
    public int BookingHorizonDays { get; set; } = 90;
}