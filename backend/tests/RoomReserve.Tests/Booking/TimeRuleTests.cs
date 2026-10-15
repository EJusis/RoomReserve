using RoomReserve.Application.Reservations.Booking;
using RoomReserve.Application.Reservations.Booking.Rules;

namespace RoomReserve.Tests.Booking;

// Covers R1, R4, and R6 booking rules

public class TimeRuleTests
{
    private readonly ValidTimeRangeRule _timeRange = new(TestData.Clock());
    private readonly MaxDurationRule _maxDuration = new(TestData.Settings(maxDays: 7));
    private readonly BookingHorizonRule _horizon = new(TestData.Settings(horizonDays: 90), TestData.Clock());

    [Fact]
    public async Task Start_in_the_past_is_rejected()
    {
        var error = await _timeRange.CheckAsync(TestData.NewRequest(start: TestData.Now.AddMinutes(-1)), default);
        Assert.Equal(BookingErrorCodes.StartInPast, error?.Code);
    }

    [Fact]
    public async Task End_before_start_is_rejected()
    {
        var error = await _timeRange.CheckAsync(TestData.NewRequest(duration: TimeSpan.FromHours(-1)), default);
        Assert.Equal(BookingErrorCodes.InvalidTimeRange, error?.Code);
    }

    [Fact]
    public async Task Future_start_with_positive_duration_is_allowed()
    {
        Assert.Null(await _timeRange.CheckAsync(TestData.NewRequest(), default));
    }

    [Fact]
    public async Task Exactly_seven_days_is_allowed()
    {
        var error = await _maxDuration.CheckAsync(TestData.NewRequest(duration: TimeSpan.FromDays(7)), default);
        Assert.Null(error);
    }

    [Fact]
    public async Task Longer_than_seven_days_is_rejected()
    {
        var request = TestData.NewRequest(duration: TimeSpan.FromDays(7) + TimeSpan.FromMinutes(1));
        var error = await _maxDuration.CheckAsync(request, default);
        Assert.Equal(BookingErrorCodes.DurationTooLong, error?.Code);
    }

    [Fact]
    public async Task Start_exactly_at_horizon_is_allowed()
    {
        var error = await _horizon.CheckAsync(TestData.NewRequest(start: TestData.Now.AddDays(90)), default);
        Assert.Null(error);
    }

    [Fact]
    public async Task Start_beyond_horizon_is_rejected()
    {
        var request = TestData.NewRequest(start: TestData.Now.AddDays(90).AddMinutes(1));
        var error = await _horizon.CheckAsync(request, default);
        Assert.Equal(BookingErrorCodes.BeyondBookingHorizon, error?.Code);
    }
}