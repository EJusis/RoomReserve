using RoomReserve.Domain.Enums;

namespace RoomReserve.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public int Floor { get; set; }
    public required string Location { get; set; }
    public Equipment Equipment { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Reservation> Reservations { get; set; } = [];
}