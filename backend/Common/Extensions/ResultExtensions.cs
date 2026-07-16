using backend.Common.Responses;
using backend.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace backend.Common.Extensions;

public static class ResultExtensions
{
    public static ActionResult<ApiResponse<T>> ToActionResult<T>(
        this Result<T> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return new ObjectResult(new ApiResponse<T>(true, result.Value))
            {
                StatusCode = successStatusCode
            };
        }

        return new ObjectResult(new ApiResponse<T>(
            success: false,
            data: default,
            message: result.Error))
        {
            StatusCode = MapErrorToStatusCode(result.Error)
        };
    }

    private static int MapErrorToStatusCode(string? error)
    {
        if (string.IsNullOrWhiteSpace(error))
            return StatusCodes.Status400BadRequest;

        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status404NotFound;

        if (error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status401Unauthorized;

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            return StatusCodes.Status409Conflict;

        return StatusCodes.Status400BadRequest;
    }
}
