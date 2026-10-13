using Microsoft.EntityFrameworkCore;
using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Infrastructure.Persistence;

public class DbSeeder(AppDbContext db, TimeProvider time)
{
    public async Task SeedAsync()
    {
        if (await db.Rooms.AnyAsync()) return;

        // Rooms
        var baltic   = new Room { Name = "Baltic",   Capacity = 12, Floor = 2, Location = "East wing",       Equipment = Equipment.Projector | Equipment.VideoConferencing | Equipment.Whiteboard };
        var nemunas  = new Room { Name = "Nemunas",  Capacity = 6,  Floor = 2, Location = "East wing",       Equipment = Equipment.TvScreen | Equipment.Whiteboard };
        var neris    = new Room { Name = "Neris",    Capacity = 4,  Floor = 2, Location = "West wing",       Equipment = Equipment.TvScreen };
        var curonian = new Room { Name = "Curonian", Capacity = 20, Floor = 3, Location = "Conference hall", Equipment = Equipment.Projector | Equipment.TvScreen | Equipment.VideoConferencing };
        var trakai   = new Room { Name = "Trakai",   Capacity = 8,  Floor = 3, Location = "West wing",       Equipment = Equipment.TvScreen | Equipment.VideoConferencing };
        var gintaras = new Room { Name = "Gintaras", Capacity = 2,  Floor = 1, Location = "Lobby",           Equipment = Equipment.None };
        var vilnius  = new Room { Name = "Vilnius",  Capacity = 10, Floor = 1, Location = "Lobby",           Equipment = Equipment.Projector | Equipment.Whiteboard };
        var kaunas   = new Room { Name = "Kaunas",   Capacity = 6,  Floor = 4, Location = "North wing",      Equipment = Equipment.Whiteboard, IsActive = false };

        // Users
        var jonas  = new User { FirstName = "Jonas",  LastName = "Petraitis" };
        var ona    = new User { FirstName = "Ona",    LastName = "Kazlauskaitė" };
        var tomas  = new User { FirstName = "Tomas",  LastName = "Jankauskas" };
        var ruta   = new User { FirstName = "Rūta",   LastName = "Vasiliauskienė" };
        var mantas = new User { FirstName = "Mantas", LastName = "Žukauskas" };
        var egle   = new User { FirstName = "Eglė",   LastName = "Butkutė" };
        var lukas  = new User { FirstName = "Lukas",  LastName = "Paulauskas" };
        var ieva   = new User { FirstName = "Ieva",   LastName = "Urbonaitė" };
        var darius = new User { FirstName = "Darius", LastName = "Kavaliauskas" };
        var greta  = new User { FirstName = "Greta",  LastName = "Navickaitė" };

        db.Rooms.AddRange(baltic, nemunas, neris, curonian, trakai, gintaras, vilnius, kaunas);
        db.Users.AddRange(jonas, ona, tomas, ruta, mantas, egle, lukas, ieva, darius, greta);

        // Reservations are relative to "now", so the demo data is never stale.
        // Hours are UTC (07:00 UTC = 10:00 in Vilnius).
        var now = time.GetUtcNow();
        var today = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);

        Reservation Book(Room room, User organizer, string title,
                         int dayOffset, double startHourUtc, double hours, int attendees) => new()
        {
            Room = room,
            Organizer = organizer,
            Title = title,
            Start = today.AddDays(dayOffset).AddHours(startHourUtc),
            End = today.AddDays(dayOffset).AddHours(startHourUtc + hours),
            AttendeeCount = attendees
        };

        static Reservation EndedEarly(Reservation r, double afterHours)
        {
            r.Status = ReservationStatus.Completed;
            r.EndedEarlyAt = r.Start.AddHours(afterHours);
            return r;
        }

        static Reservation Cancelled(Reservation r)
        {
            r.Status = ReservationStatus.Cancelled;
            return r;
        }

        db.Reservations.AddRange(
            // Past: "Baltic" is booked a lot but used little, so the usage report has something to show
            Book(curonian, jonas, "All-hands", -14, 7, 2, 18),
            Book(baltic, ona, "Sprint planning", -10, 7, 2, 8),
            EndedEarly(Book(baltic, tomas, "Client demo", -9, 11, 1.5, 10), 0.5),
            EndedEarly(Book(baltic, ruta, "Design review", -8, 8, 3, 6), 0.75),
            Cancelled(Book(baltic, mantas, "Weekly sync", -7, 7, 1, 5)),
            EndedEarly(Book(baltic, egle, "Architecture workshop", -6, 7, 4, 9), 1),
            Book(nemunas, lukas, "1:1", -5, 9, 0.5, 2),
            Book(trakai, ieva, "Vendor call", -4, 12, 1, 4),
            Book(vilnius, darius, "Retrospective", -3, 13, 1.5, 7),
            Book(neris, greta, "Interview", -2, 8, 1, 3),

            // In progress right now, so "end early" can be demoed
            new Reservation
            {
                Room = trakai, Organizer = tomas, Title = "Incident review",
                Start = now.AddMinutes(-30), End = now.AddMinutes(60), AttendeeCount = 5
            },

            // Upcoming
            Book(baltic, ona, "Sprint planning", 1, 7, 2, 8),
            Book(curonian, jonas, "Town hall", 2, 11, 2, 20),
            Book(nemunas, ruta, "Product sync", 2, 8, 1, 5),
            Book(vilnius, mantas, "Onboarding training", 3, 6, 8, 10),
            Book(gintaras, egle, "Phone interview", 3, 9, 0.5, 1),
            Cancelled(Book(baltic, lukas, "Offsite prep", 5, 7, 1, 6)),
            Book(trakai, ieva, "Partner call", 7, 12, 1, 6),
            Book(neris, darius, "Code review", 8, 10, 1, 4)
        );

        await db.SaveChangesAsync();
    }
}