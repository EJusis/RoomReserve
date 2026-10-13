using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Rooms;

public class RoomService(IAppDbContext db) : IRoomService
{
    public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(RoomFilter filter, CancellationToken ct = default)
    {
        var query = db.Rooms.AsNoTracking();

        if (filter.MinCapacity is int minCapacity)
            query = query.Where(r => r.Capacity >= minCapacity);

        if (filter.Floor is int floor)
            query = query.Where(r => r.Floor == floor);

        if (filter.Equipment is { Count: > 0 })
        {
            // Room must have ALL selected equipment
            var required = filter.Equipment.Aggregate(Equipment.None, (all, e) => all | e);
            query = query.Where(r => (r.Equipment & required) == required);
        }

        var rooms = await query
            .OrderBy(r => r.Floor)
            .ThenBy(r => r.Name)
            .ToListAsync(ct);

        return rooms.Select(r => r.ToDto()).ToList();
    }
}