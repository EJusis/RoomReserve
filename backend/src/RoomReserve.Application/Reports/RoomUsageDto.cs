namespace RoomReserve.Application.Reports;

public record RoomUsageDto(
    int RoomId,
    string RoomName,
    int Floor,
    int Capacity,
    bool IsActive,
    int MeetingCount,
    int EndedEarlyCount,
    int CancelledCount,
    double BookedHours,
    double UsedHours,
    double? Utilization,
    bool IsUnderused);

public record RoomUsageReportDto(
    DateTimeOffset From,
    DateTimeOffset To,
    double TotalBookedHours,
    double TotalUsedHours,
    double? OverallUtilization,
    double LowUtilizationThreshold,
    IReadOnlyList<RoomUsageDto> Rooms);