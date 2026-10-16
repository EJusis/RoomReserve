using Microsoft.AspNetCore.Mvc;
using RoomReserve.Domain.Errors;

namespace RoomReserve.Api.Errors;

public static class ErrorResponses
{
    public static ObjectResult From(IReadOnlyList<Error> errors, HttpContext http)
    {
        var status =
            errors.Any(e => e.Type == ErrorType.NotFound) ? StatusCodes.Status404NotFound :
            errors.All(e => e.Type == ErrorType.Conflict) ? StatusCodes.Status409Conflict :
            StatusCodes.Status400BadRequest;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = status switch
            {
                StatusCodes.Status404NotFound => "Not found",
                StatusCodes.Status409Conflict => "Conflict",
                _ => "Request could not be processed"
            },
            Detail = errors.Count == 1 ? errors[0].Message : $"{errors.Count} problems were found.",
            Instance = http.Request.Path
        };

        problem.Extensions["errors"] = errors.Select(e => new { code = e.Code, message = e.Message });
        problem.Extensions["traceId"] = http.TraceIdentifier;

        return new ObjectResult(problem) { StatusCode = status };
    }

    public static IActionResult FromModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(e => new Error(
                "INVALID_INPUT",
                string.IsNullOrWhiteSpace(e.ErrorMessage) ? $"Invalid value for '{entry.Key}'." : e.ErrorMessage,
                ErrorType.Validation)))
            .ToList();

        return From(errors, context.HttpContext);
    }
}