using Microsoft.AspNetCore.Mvc;
using RoomReserve.Application.Users;

namespace RoomReserve.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetUsers(CancellationToken ct)
        => Ok(await users.GetUsersAsync(ct));
}