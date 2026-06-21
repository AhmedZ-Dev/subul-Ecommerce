using System.Net;
using System.Text.Json;
using backend.Common.Responses;
using FluentValidation;

namespace backend.Common.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                success: false,
                data: null,
                message: "Validation failed",
                errors: ex.Errors.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>(
                success: false,
                data: null,
                message: "An unexpected error occurred.");

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
