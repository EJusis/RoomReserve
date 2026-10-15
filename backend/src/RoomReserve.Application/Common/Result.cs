using RoomReserve.Domain.Errors;

namespace RoomReserve.Application.Common;

public class Result<T>
{
    public T? Value { get; }
    public IReadOnlyList<Error> Errors { get; }
    public bool IsSuccess => Errors.Count == 0;

    private Result(T? value, IReadOnlyList<Error> errors)
    {
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value, []);
    public static Result<T> Failure(params Error[] errors) => new(default, errors);
    public static Result<T> Failure(IEnumerable<Error> errors) => new(default, errors.ToList());
}