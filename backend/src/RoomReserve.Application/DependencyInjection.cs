using Microsoft.Extensions.DependencyInjection;
using RoomReserve.Application.Rooms;
using RoomReserve.Application.Users;

namespace RoomReserve.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}