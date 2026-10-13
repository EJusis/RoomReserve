using Microsoft.AspNetCore.Mvc;
using RoomReserve.Application.Rooms;

namespace RoomReserve.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomService rooms) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetRooms(
        [FromQuery] RoomFilter filter, CancellationToken ct)
        => Ok(await rooms.GetRoomsAsync(filter, ct));
}