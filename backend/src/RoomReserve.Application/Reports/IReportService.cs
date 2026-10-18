using RoomReserve.Application.Common;

namespace RoomReserve.Application.Reports;

public interface IReportService
{
    Task<Result<RoomUsageReportDto>> GetRoomUsageAsync(
        DateTimeOffset? from, DateTimeOffset? to, CancellationToken ct = default);
}