using RoomReserve.Application.Reports;
using RoomReserve.Domain.Entities;
using RoomReserve.Domain.Enums;
using RoomReserve.Tests.Booking;

namespace RoomReserve.Tests.Reports;

public class RoomUsageCalculatorTests
{
    private static readonly DateTimeOffset From = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset To = From.AddDays(30);
    private static readonly ReportOptions Settings = new() { LowUtilizationThreshold = 0.6, MinMeetingsToFlag = 3 };
    private static readonly Room TestRoom = TestData.NewRoom();

    /// <summary>A meeting starting `startHours` after the period begins.</summary>
    private static Reservation Meeting(double startHours, double hours, double? endedAfterHours = null, bool cancelled = false)
    {
        var start = From.AddHours(startHours);
        return new Reservation
        {
            RoomId = TestRoom.Id,
            OrganizerId = 1,
            Title = "Meeting",
            AttendeeCount = 2,
            Start = start,
            End = start.AddHours(hours),
            EndedEarlyAt = endedAfterHours is { } h ? start.AddHours(h) : null,
            Status = cancelled ? ReservationStatus.Cancelled
                   : endedAfterHours is null ? ReservationStatus.Booked
                   : ReservationStatus.Completed
        };
    }

    private static RoomUsageDto Calculate(params Reservation[] reservations) =>
        RoomUsageCalculator.Calculate(TestRoom, reservations, From, To, Settings);

    [Fact]
    public void Meeting_used_in_full_has_full_utilization()
    {
        var usage = Calculate(Meeting(24, hours: 2));

        Assert.Equal(2, usage.BookedHours);
        Assert.Equal(2, usage.UsedHours);
        Assert.Equal(1.0, usage.Utilization);
    }

    [Fact]
    public void Ending_early_counts_only_the_used_part()
    {
        var usage = Calculate(Meeting(24, hours: 4, endedAfterHours: 1));

        Assert.Equal(4, usage.BookedHours);
        Assert.Equal(1, usage.UsedHours);
        Assert.Equal(0.25, usage.Utilization);
        Assert.Equal(1, usage.EndedEarlyCount);
    }

    [Fact]
    public void Cancelled_meetings_are_counted_but_not_as_booked_time()
    {
        var usage = Calculate(Meeting(24, hours: 2, cancelled: true));

        Assert.Equal(1, usage.CancelledCount);
        Assert.Equal(0, usage.MeetingCount);
        Assert.Equal(0, usage.BookedHours);
        Assert.Null(usage.Utilization);
    }

    [Fact]
    public void Time_outside_the_period_is_ignored()
    {
        var usage = Calculate(Meeting(-1, hours: 2)); // starts an hour before the period

        Assert.Equal(1, usage.BookedHours);
    }

    [Fact]
    public void Often_booked_but_rarely_used_room_is_flagged()
    {
        var usage = Calculate(
            Meeting(24, hours: 4, endedAfterHours: 1),
            Meeting(48, hours: 4, endedAfterHours: 1),
            Meeting(72, hours: 4, endedAfterHours: 1));

        Assert.True(usage.IsUnderused);
    }

    [Fact]
    public void Too_few_meetings_are_not_flagged()
    {
        var usage = Calculate(
            Meeting(24, hours: 4, endedAfterHours: 1),
            Meeting(48, hours: 4, endedAfterHours: 1));

        Assert.False(usage.IsUnderused);
    }

    [Fact]
    public void Room_without_bookings_has_no_utilization()
    {
        var usage = Calculate();

        Assert.Null(usage.Utilization);
        Assert.False(usage.IsUnderused);
    }
}