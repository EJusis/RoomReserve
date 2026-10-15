namespace RoomReserve.Domain.Errors;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict
}

public record Error(string Code, string Message, ErrorType Type);