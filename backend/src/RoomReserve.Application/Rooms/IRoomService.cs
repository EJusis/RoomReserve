namespace RoomReserve.Application.Rooms;

public interface IRoomService
{
    Task<IReadOnlyList<RoomDto>> GetRoomsAsync(RoomFilter filter, CancellationToken ct = default);
}