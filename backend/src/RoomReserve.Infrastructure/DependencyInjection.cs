using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomReserve.Application.Common;
using RoomReserve.Infrastructure.Persistence;

namespace RoomReserve.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("RoomReserve"));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<DbSeeder>();
        return services;
    }
}