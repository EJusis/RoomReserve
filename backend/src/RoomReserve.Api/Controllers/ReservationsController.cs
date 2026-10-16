using Microsoft.AspNetCore.Mvc;
using RoomReserve.Api.Errors;
using RoomReserve.Application.Common;
using RoomReserve.Application.Reservations;

namespace RoomReserve.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(IReservationService reservations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReservationDto>>> GetReservations(
        [FromQuery] ReservationFilter filter, CancellationToken ct)
        => Ok(await reservations.GetReservationsAsync(filter, ct));

    [HttpPost]
    [ProducesResponseType<ReservationDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationDto>> Create(
        CreateReservationRequest request, CancellationToken ct)
    {
        var result = await reservations.CreateAsync(request, ct);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ErrorResponses.From(result.Errors, HttpContext);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ReservationDto>> Cancel(int id, CancellationToken ct)
        => ToResponse(await reservations.CancelAsync(id, ct));

    [HttpPost("{id:int}/end-early")]
    public async Task<ActionResult<ReservationDto>> EndEarly(int id, CancellationToken ct)
        => ToResponse(await reservations.EndEarlyAsync(id, ct));

    private ActionResult<ReservationDto> ToResponse(Result<ReservationDto> result) =>
        result.IsSuccess
            ? Ok(result.Value)
            : ErrorResponses.From(result.Errors, HttpContext);
}