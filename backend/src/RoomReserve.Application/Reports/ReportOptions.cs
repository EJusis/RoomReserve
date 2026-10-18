namespace RoomReserve.Application.Reports;

public class ReportOptions
{
    public const string SectionName = "Reports";

    /// <summary>Rooms used less than this share of their booked time are flagged (0..1).</summary>
    public double LowUtilizationThreshold { get; set; } = 0.6;

    /// <summary>A room needs at least this many meetings before it can be flagged.</summary>
    public int MinMeetingsToFlag { get; set; } = 3;

    /// <summary>Report period when no dates are given.</summary>
    public int DefaultPeriodDays { get; set; } = 30;
}