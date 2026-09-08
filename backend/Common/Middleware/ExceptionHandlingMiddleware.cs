using System.Net;
using System.Text.Json;
using backend.Common.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace backend.Common.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// MVC serialises responses as camelCase, but this middleware writes its own
    /// JSON and used the serializer defaults — so every error body it produced
    /// came back PascalCase, and clients reading `data.success` or `data.errors`
    /// saw undefined. Match MVC so one error shape reaches the frontends.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Postgres constraint violations that are the caller's fault, not the
    /// server's. Without this they fell through to the generic handler and came
    /// back as 500 — which tells clients and proxies to retry a request that can
    /// only ever fail, and buries real outages under routine input errors.
    ///
    /// The messages are deliberately generic: SQLSTATE 22001 does not carry the
    /// column name, and the ones that do carry constraint and table names must
    /// not leak them to the caller. Details go to the log.
    /// </summary>
    private static readonly Dictionary<string, (int StatusCode, string Message)> PostgresErrorMap = new()
    {
        // string_data_right_truncation
        ["22001"] = (StatusCodes.Status400BadRequest,
            "One of the submitted fields is longer than allowed."),
        // not_null_violation
        ["23502"] = (StatusCodes.Status400BadRequest,
            "A required field is missing."),
        // foreign_key_violation
        ["23503"] = (StatusCodes.Status400BadRequest,
            "A referenced record does not exist."),
        // unique_violation
        ["23505"] = (StatusCodes.Status409Conflict,
            "A record with these values already exists."),
        // check_violation
        ["23514"] = (StatusCodes.Status400BadRequest,
            "One of the submitted values is not allowed."),
        // numeric_value_out_of_range
        ["22003"] = (StatusCodes.Status400BadRequest,
            "One of the submitted numbers is out of range."),
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation failed",
                ex.Errors.Select(e => e.ErrorMessage).ToArray());
        }
        catch (Exception ex) when (TryMapPostgresError(ex, out var mapped))
        {
            // Logged at warning, not error: a rejected write is a client mistake
            // and should not page anyone.
            logger.LogWarning(
                ex,
                "Database rejected the write for {Method} {Path} (SQLSTATE {SqlState})",
                context.Request.Method,
                context.Request.Path,
                mapped.SqlState);

            await WriteAsync(context, mapped.StatusCode, mapped.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            await WriteAsync(
                context,
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// EF wraps the driver exception in a <see cref="DbUpdateException"/> on
    /// SaveChanges, but a raw command surfaces the <see cref="PostgresException"/>
    /// directly — both shapes are matched.
    /// </summary>
    private static bool TryMapPostgresError(
        Exception exception,
        out (int StatusCode, string Message, string SqlState) mapped)
    {
        var postgresException = exception as PostgresException
            ?? exception.InnerException as PostgresException;

        if (postgresException is not null &&
            PostgresErrorMap.TryGetValue(postgresException.SqlState, out var entry))
        {
            mapped = (entry.StatusCode, entry.Message, postgresException.SqlState);
            return true;
        }

        mapped = default;
        return false;
    }

    private static Task WriteAsync(
        HttpContext context,
        int statusCode,
        string message,
        string[]? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>(
            success: false,
            data: null,
            message: message,
            errors: errors);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
