using Microsoft.EntityFrameworkCore;
using RoomReserve.Domain.Entities;

namespace RoomReserve.Application.Common;

public interface IAppDbContext
{
    DbSet<Room> Rooms { get; }
    DbSet<User> Users { get; }
    DbSet<Reservation> Reservations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}