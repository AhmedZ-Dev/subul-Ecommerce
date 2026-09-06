using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Common.Responses;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace backend.Common.RateLimiting;

public sealed class RedisRateLimitOptions
{
    public const string SectionName = "RedisRateLimit";

    public bool Enabled { get; init; } = true;
    public int PermitLimit { get; init; } = 120;
    public int WindowSeconds { get; init; } = 60;
    public int LoginPermitLimit { get; init; } = 10;
    public int LoginWindowSeconds { get; init; } = 300;
}

public sealed class RedisRateLimiter(IConnectionMultiplexer connectionMultiplexer)
{
    private const string FixedWindowScript = """
        local count = redis.call('INCR', KEYS[1])
        if count == 1 then
            redis.call('PEXPIRE', KEYS[1], ARGV[1])
        end
        local ttl = redis.call('PTTL', KEYS[1])
        return {count, ttl}
        """;

    public async Task<RedisRateLimitDecision> CheckAsync(
        string policy,
        string clientIdentifier,
        int permitLimit,
        TimeSpan window,
        CancellationToken cancellationToken)
    {
        var identifierHash = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(clientIdentifier)));
        RedisKey key = $"subul:rate-limit:{policy}:{identifierHash}";
        var windowMilliseconds = Math.Max(1L, (long)window.TotalMilliseconds);

        var redisResult = await connectionMultiplexer
            .GetDatabase()
            .ScriptEvaluateAsync(
                FixedWindowScript,
                [key],
                [windowMilliseconds])
            .WaitAsync(cancellationToken);

        var values = (RedisResult[]?)redisResult
            ?? throw new RedisException("Redis returned an invalid rate-limit response.");
        var requestCount = (long)values[0];
        var retryAfterMilliseconds = Math.Max(0L, (long)values[1]);

        return new RedisRateLimitDecision(
            IsAllowed: requestCount <= permitLimit,
            Remaining: Math.Max(0, permitLimit - requestCount),
            RetryAfter: TimeSpan.FromMilliseconds(retryAfterMilliseconds));
    }
}

public sealed record RedisRateLimitDecision(
    bool IsAllowed,
    long Remaining,
    TimeSpan RetryAfter);

public sealed class RedisRateLimitMiddleware(
    RequestDelegate next,
    IOptions<RedisRateLimitOptions> options,
    ILogger<RedisRateLimitMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!options.Value.Enabled ||
            HttpMethods.IsOptions(context.Request.Method) ||
            !context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var rateLimiter = context.RequestServices.GetService<RedisRateLimiter>();
        if (rateLimiter is null)
        {
            await next(context);
            return;
        }

        var isLoginRequest = HttpMethods.IsPost(context.Request.Method) &&
            string.Equals(context.Request.Path.Value, "/api/auth/login", StringComparison.OrdinalIgnoreCase);
        var permitLimit = isLoginRequest
            ? options.Value.LoginPermitLimit
            : options.Value.PermitLimit;
        var windowSeconds = isLoginRequest
            ? options.Value.LoginWindowSeconds
            : options.Value.WindowSeconds;
        var policy = isLoginRequest ? "login" : "api";
        var clientIdentifier = GetClientIdentifier(context);

        RedisRateLimitDecision decision;
        try
        {
            decision = await rateLimiter.CheckAsync(
                policy,
                clientIdentifier,
                permitLimit,
                TimeSpan.FromSeconds(windowSeconds),
                context.RequestAborted);
        }
        catch (Exception exception) when (exception is RedisException or TimeoutException)
        {
            logger.LogWarning(exception, "Redis rate limiter is unavailable; allowing the request");
            await next(context);
            return;
        }

        if (decision.IsAllowed)
        {
            context.Response.OnStarting(() =>
            {
                SetRateLimitHeaders(context.Response, permitLimit, decision.Remaining);
                return Task.CompletedTask;
            });

            await next(context);
            return;
        }

        var retryAfterSeconds = Math.Max(1, (int)Math.Ceiling(decision.RetryAfter.TotalSeconds));
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        SetRateLimitHeaders(context.Response, permitLimit, decision.Remaining);
        context.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);

        var response = new ApiResponse<object>(
            success: false,
            data: null,
            message: "Too many requests. Please try again later.");

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }

    private static void SetRateLimitHeaders(HttpResponse response, int permitLimit, long remaining)
    {
        response.Headers["X-RateLimit-Limit"] = permitLimit.ToString(CultureInfo.InvariantCulture);
        response.Headers["X-RateLimit-Remaining"] = remaining.ToString(CultureInfo.InvariantCulture);
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            context.User.FindFirstValue("sub");

        if (!string.IsNullOrWhiteSpace(userId))
            return $"user:{userId}";

        return $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }
}
