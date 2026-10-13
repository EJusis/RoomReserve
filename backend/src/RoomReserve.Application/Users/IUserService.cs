namespace RoomReserve.Application.Users;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default);
}