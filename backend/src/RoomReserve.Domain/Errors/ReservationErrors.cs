namespace RoomReserve.Domain.Errors;

public static class ReservationErrors
{
    public static Error NotFound(int id) =>
        new("RESERVATION_NOT_FOUND", $"Reservation {id} was not found.", ErrorType.NotFound);

    public static Error RoomNotFound(int id) =>
        new("ROOM_NOT_FOUND", $"Room {id} was not found.", ErrorType.NotFound);

    public static Error OrganizerNotFound(int id) =>
        new("ORGANIZER_NOT_FOUND", $"User {id} was not found.", ErrorType.NotFound);

    public static readonly Error AlreadyCancelled =
        new("ALREADY_CANCELLED", "This reservation is already cancelled.", ErrorType.Conflict);

    public static readonly Error AlreadyCompleted =
        new("ALREADY_COMPLETED", "This meeting has already ended.", ErrorType.Conflict);

    public static readonly Error AlreadyStarted =
        new("ALREADY_STARTED", "The meeting has already started. End it early instead of cancelling.", ErrorType.Conflict);

    public static readonly Error NotStarted =
        new("NOT_STARTED", "The meeting has not started yet. Cancel it instead of ending it early.", ErrorType.Conflict);
}