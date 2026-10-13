using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Rooms;

public record RoomFilter
{
    public int? MinCapacity { get; init; }
    public int? Floor { get; init; }
    public List<Equipment>? Equipment { get; init; }
}