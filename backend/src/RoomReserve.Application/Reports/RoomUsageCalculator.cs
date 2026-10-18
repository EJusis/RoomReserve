using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;

namespace RoomReserve.Application.Reports;

/// <summary>Pure calculation of booked vs. used time for one room within [from, to).</summary>
public static class RoomUsageCalculator
{
    public static RoomUsageDto Calculate(
        Room room,
        IEnumerable<Reservation> reservations,
        DateTimeOffset from,
        DateTimeOffset to,
        ReportOptions options)
    {
        int meetings = 0, endedEarly = 0, cancelled = 0;
        var booked = TimeSpan.Zero;
        var used = TimeSpan.Zero;

        foreach (var r in reservations)
        {
            var bookedPart = Overlap(r.Start, r.End, from, to);
            if (bookedPart <= TimeSpan.Zero)
                continue; // outside the period

            if (r.Status == ReservationStatus.Cancelled)
            {
                cancelled++;
                continue; // a cancellation freed the room: neither booked nor used time
            }

            meetings++;
            if (r.EndedEarlyAt is not null)
                endedEarly++;

            booked += bookedPart;
            used += Overlap(r.Start, r.EndedEarlyAt ?? r.End, from, to);
        }

        double? utilization = booked > TimeSpan.Zero ? used / booked : null;
        double? roundedUtilization = utilization is null ? null : Math.Round(utilization.Value, 3);

        // "Often booked but rarely used": enough meetings, and a low share of them actually used
        var isUnderused = meetings >= options.MinMeetingsToFlag
                          && utilization < options.LowUtilizationThreshold;

        return new RoomUsageDto(
            room.Id, room.Name, room.Floor, room.Capacity, room.IsActive,
            meetings, endedEarly, cancelled,
            Math.Round(booked.TotalHours, 2),
            Math.Round(used.TotalHours, 2),
            roundedUtilization,
            isUnderused);
    }

    /// <summary>Length of the part of [start, end) that falls inside [from, to).</summary>
    private static TimeSpan Overlap(DateTimeOffset start, DateTimeOffset end, DateTimeOffset from, DateTimeOffset to)
    {
        var overlapStart = start > from ? start : from;
        var overlapEnd = end < to ? end : to;
        return overlapEnd > overlapStart ? overlapEnd - overlapStart : TimeSpan.Zero;
    }
}