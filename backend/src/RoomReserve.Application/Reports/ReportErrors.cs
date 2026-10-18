using RoomReserve.Domain.Errors;

namespace RoomReserve.Application.Reports;

public static class ReportErrors
{
    public static readonly Error InvalidPeriod =
        new("INVALID_PERIOD", "The start of the period must be before its end.", ErrorType.Validation);
}