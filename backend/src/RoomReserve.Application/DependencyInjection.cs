using Microsoft.Extensions.DependencyInjection;
using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;
using RoomReserve.Application.Rooms;
using RoomReserve.Application.Users;
using RoomReserve.Application.Reservations;
using RoomReserve.Application.Reports;

namespace RoomReserve.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IUserService, UserService>();

        // Booking rules – executed in registration order
        services.AddScoped<IBookingRule, ValidTimeRangeRule>();
        services.AddScoped<IBookingRule, MaxDurationRule>();
        services.AddScoped<IBookingRule, BookingHorizonRule>();
        services.AddScoped<IBookingRule, ActiveRoomRule>();
        services.AddScoped<IBookingRule, CapacityRule>();
        services.AddScoped<IBookingRule, RoomAvailabilityRule>();
        services.AddScoped<IBookingRule, OrganizerAvailabilityRule>();
        services.AddScoped<BookingValidator>();

        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}