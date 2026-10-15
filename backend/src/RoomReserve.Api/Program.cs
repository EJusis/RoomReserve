using System.Text.Json.Serialization;
using RoomReserve.Application;
using RoomReserve.Infrastructure;
using RoomReserve.Infrastructure.Persistence;
using RoomReserve.Application.Reservations.Booking;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddApplication();
builder.Services
    .AddOptions<BookingOptions>()
    .Bind(builder.Configuration.GetSection(BookingOptions.SectionName))
    .Validate(o => o.MaxDurationDays > 0 && o.BookingHorizonDays > 0,
        "Booking limits must be positive.")
    .ValidateOnStart();
builder.Services.AddInfrastructure();

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();