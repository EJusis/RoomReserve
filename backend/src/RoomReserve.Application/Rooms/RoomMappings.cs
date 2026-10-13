using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Rooms;

public static class RoomMappings
{
    public static RoomDto ToDto(this Room room) => new(
        room.Id,
        room.Name,
        room.Capacity,
        room.Floor,
        room.Location,
        SplitFlags(room.Equipment),
        room.IsActive);

    // Projector | Whiteboard  →  [Projector, Whiteboard]
    private static IReadOnlyList<Equipment> SplitFlags(Equipment equipment) =>
        Enum.GetValues<Equipment>()
            .Where(e => e != Equipment.None && equipment.HasFlag(e))
            .ToList();
}