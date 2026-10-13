using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;
using RoomReserve.Domain.Entities;

namespace RoomReserve.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(b =>
        {
            b.HasOne(r => r.Room)
             .WithMany(room => room.Reservations)
             .HasForeignKey(r => r.RoomId);

            b.HasOne(r => r.Organizer)
             .WithMany(u => u.Reservations)
             .HasForeignKey(r => r.OrganizerId);
        });
    }
}