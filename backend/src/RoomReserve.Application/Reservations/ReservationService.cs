using Microsoft.EntityFrameworkCore;
using RoomReserve.Application.Common;
using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;
using RoomReserve.Domain.Errors;

namespace RoomReserve.Application.Reservations;

public class ReservationService(IAppDbContext db, BookingValidator validator, TimeProvider time)
    : IReservationService
{
    public async Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(
        ReservationFilter filter, CancellationToken ct = default)
    {
        var now = time.GetUtcNow();

        IQueryable<Reservation> query = db.Reservations
            .AsNoTracking()
            .Include(r => r.Room)
            .Include(r => r.Organizer);

        if (filter.RoomId is int roomId)
            query = query.Where(r => r.RoomId == roomId);

        if (filter.OrganizerId is int organizerId)
            query = query.Where(r => r.OrganizerId == organizerId);

        if (filter.From is { } from)
            query = query.Where(r => r.End > from);

        if (filter.To is { } to)
            query = query.Where(r => r.Start < to);

        // Status is partly derived from time, so filter on the same logic as GetStatus()
        query = filter.Status switch
        {
            ReservationStatus.Booked => query.Where(r =>
                r.Status == ReservationStatus.Booked && r.End > now),
            ReservationStatus.Completed => query.Where(r =>
                r.Status == ReservationStatus.Completed ||
                (r.Status == ReservationStatus.Booked && r.End <= now)),
            ReservationStatus.Cancelled => query.Where(r =>
                r.Status == ReservationStatus.Cancelled),
            _ => query
        };

        var reservations = await query.OrderBy(r => r.Start).ToListAsync(ct);
        return reservations.Select(r => r.ToDto(now)).ToList();
    }

    public async Task<Result<ReservationDto>> CreateAsync(
        CreateReservationRequest request, CancellationToken ct = default)
    {
        var room = await db.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);
        if (room is null)
            return Result<ReservationDto>.Failure(ReservationErrors.RoomNotFound(request.RoomId));

        var organizer = await db.Users.FirstOrDefaultAsync(u => u.Id == request.OrganizerId, ct);
        if (organizer is null)
            return Result<ReservationDto>.Failure(ReservationErrors.OrganizerNotFound(request.OrganizerId));

        var start = request.Start.ToUniversalTime();
        var end = request.End.ToUniversalTime();

        var bookingErrors = await validator.ValidateAsync(
            new BookingRequest(room, organizer.Id, start, end, request.AttendeeCount), ct);

        if (bookingErrors.Count > 0)
            return Result<ReservationDto>.Failure(bookingErrors.Select(ToError));

        var reservation = new Reservation
        {
            Room = room,
            Organizer = organizer,
            Title = request.Title.Trim(),
            Start = start,
            End = end,
            AttendeeCount = request.AttendeeCount
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync(ct);

        return Result<ReservationDto>.Success(reservation.ToDto(time.GetUtcNow()));
    }

    public Task<Result<ReservationDto>> CancelAsync(int id, CancellationToken ct = default) =>
        ChangeStateAsync(id, (reservation, now) => reservation.Cancel(now), ct);

    public Task<Result<ReservationDto>> EndEarlyAsync(int id, CancellationToken ct = default) =>
        ChangeStateAsync(id, (reservation, now) => reservation.EndEarly(now), ct);

    // Shared flow: load → let the entity apply its own rules → save
    private async Task<Result<ReservationDto>> ChangeStateAsync(
        int id, Func<Reservation, DateTimeOffset, Error?> action, CancellationToken ct)
    {
        var reservation = await db.Reservations
            .Include(r => r.Room)
            .Include(r => r.Organizer)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (reservation is null)
            return Result<ReservationDto>.Failure(ReservationErrors.NotFound(id));

        var now = time.GetUtcNow();

        if (action(reservation, now) is { } error)
            return Result<ReservationDto>.Failure(error);

        await db.SaveChangesAsync(ct);
        return Result<ReservationDto>.Success(reservation.ToDto(now));
    }

    // Overlaps are conflicts with existing data; everything else is invalid input
    private static Error ToError(BookingError e) => new(
        e.Code,
        e.Message,
        e.Code is BookingErrorCodes.RoomOverlap or BookingErrorCodes.OrganizerOverlap
            ? ErrorType.Conflict
            : ErrorType.Validation);
}