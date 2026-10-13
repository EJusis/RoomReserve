using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Rooms;

public record RoomDto(
    int Id,
    string Name,
    int Capacity,
    int Floor,
    string Location,
    IReadOnlyList<Equipment> Equipment,
    bool IsActive);