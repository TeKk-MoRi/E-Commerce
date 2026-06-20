using Catalog.Application.Common;
using Catalog.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Extensions;

public static class ResultExtensions
{
    public static ProblemDetails MapToProblemDetails(this Result result)
    {
        var status = result.Error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ProblemDetails
        {
            Status = status,
            Title = GetTitle(result.Error.Type),
            Detail = result.Error.Message,
            Type = GetType(result.Error.Type),
            Extensions =
            {
                ["errorCode"] = result.Error.Code
            }
        };
    }

    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Validation error",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Server error"
        };
    }

    private static string GetType(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            ErrorType.Forbidden => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };
    }
}