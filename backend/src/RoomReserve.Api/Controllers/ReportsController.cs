using Microsoft.AspNetCore.Mvc;
using RoomReserve.Api.Errors;
using RoomReserve.Application.Reports;

namespace RoomReserve.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IReportService reports) : ControllerBase
{
    /// <summary>Booked vs. actually used time per room. Defaults to the last 30 days.</summary>
    [HttpGet("room-usage")]
    [ProducesResponseType<RoomUsageReportDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomUsageReportDto>> GetRoomUsage(
        [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken ct)
    {
        var result = await reports.GetRoomUsageAsync(from, to, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : ErrorResponses.From(result.Errors, HttpContext);
    }
}