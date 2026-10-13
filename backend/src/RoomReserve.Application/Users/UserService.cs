using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Users;

public class UserService(IAppDbContext db, TimeProvider time) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
    {
        var now = time.GetUtcNow();

        return await db.Users
            .AsNoTracking()
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new UserDto(
                u.Id,
                u.FirstName,
                u.LastName,
                // Active = booked and not yet over (not cancelled, not ended early, not past)
                u.Reservations.Count(r => r.Status == ReservationStatus.Booked && r.End > now)))
            .ToListAsync(ct);
    }
}