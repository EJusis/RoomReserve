using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoomReserve.Application.Common;

namespace RoomReserve.Application.Reports;

public class ReportService(IAppDbContext db, TimeProvider time, IOptions<ReportOptions> options)
    : IReportService
{
    public async Task<Result<RoomUsageReportDto>> GetRoomUsageAsync(
        DateTimeOffset? from, DateTimeOffset? to, CancellationToken ct = default)
    {
        var settings = options.Value;
        var now = time.GetUtcNow();

        var periodTo = to ?? now;
        var periodFrom = from ?? periodTo.AddDays(-settings.DefaultPeriodDays);

        if (periodFrom >= periodTo)
            return Result<RoomUsageReportDto>.Failure(ReportErrors.InvalidPeriod);

        // Only time that has already passed can be used, so future bookings never count as "unused"
        var countUntil = periodTo < now ? periodTo : now;

        var rooms = await db.Rooms.AsNoTracking().ToListAsync(ct);

        var reservations = await db.Reservations
            .AsNoTracking()
            .Where(r => r.Start < countUntil && r.End > periodFrom)
            .ToListAsync(ct);

        var reservationsByRoom = reservations.ToLookup(r => r.RoomId);

        var usage = rooms
            .Select(room => RoomUsageCalculator.Calculate(
                room, reservationsByRoom[room.Id], periodFrom, countUntil, settings))
            .OrderBy(u => u.Utilization ?? double.MaxValue) // least-used first, unbooked rooms last
            .ThenBy(u => u.RoomName)
            .ToList();

        var totalBooked = usage.Sum(u => u.BookedHours);
        var totalUsed = usage.Sum(u => u.UsedHours);
        double? overall = totalBooked > 0 ? Math.Round(totalUsed / totalBooked, 3) : null;

        return Result<RoomUsageReportDto>.Success(new RoomUsageReportDto(
            periodFrom,
            periodTo,
            Math.Round(totalBooked, 2),
            Math.Round(totalUsed, 2),
            overall,
            settings.LowUtilizationThreshold,
            usage));
    }
}