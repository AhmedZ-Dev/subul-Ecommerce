using System.Net;
using System.Net.Http.Json;
using backend.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace backend.Tests.Features.RateLimiting;

/// <summary>
/// Regression tests for the Wave B finding H-9.
///
/// The limiter used to fail open in two ways, both invisible: with no
/// ConnectionStrings:Redis it was never registered and the middleware waved
/// every request through without a log line, and on a Redis outage it logged a
/// warning and did the same. Either one silently removed the brute-force
/// protection from /api/auth/login while the app reported itself healthy.
///
/// The fix has two halves, one per failure: refuse to start when the feature is
/// enabled but unconfigured, and fail closed on login (only) when Redis cannot
/// answer at runtime.
/// </summary>
[Collection("Database")]
public class RateLimiterFailureModeTests(DatabaseFixture fixture)
{
    /// <summary>A port nothing is listening on, so every Redis call fails fast.</summary>
    private const string UnreachableRedis =
        "127.0.0.1:6399,abortConnect=false,connectTimeout=250,syncTimeout=250";

    private WebApplicationFactoryOf Configure(string? redis, bool enabled) =>
        new(fixture.ConnectionString, redis, enabled);

    // Half one: a missing connection string is now loud instead of invisible.
    [Fact]
    public void Startup_RateLimitEnabledWithoutRedis_RefusesToBoot()
    {
        using var factory = Configure(redis: null, enabled: true);

        var exception = Assert.Throws<OptionsValidationException>(() => factory.Build());

        Assert.Contains("ConnectionStrings:Redis", exception.Message, StringComparison.Ordinal);
    }

    // Turning it off stays a legitimate, explicit choice — the guard must not
    // force Redis on someone who has deliberately opted out.
    [Fact]
    public void Startup_RateLimitDisabledWithoutRedis_BootsFine()
    {
        using var factory = Configure(redis: null, enabled: false);

        var client = factory.Build();

        Assert.NotNull(client);
    }

    [Fact]
    public void Startup_RateLimitEnabledWithRedis_BootsFine()
    {
        using var factory = Configure(redis: UnreachableRedis, enabled: true);

        var client = factory.Build();

        Assert.NotNull(client);
    }

    // Half two: knocking Redis over is how an attacker would strip the
    // brute-force protection before running a password list. Login must refuse
    // rather than serve unlimited attempts.
    [Fact]
    public async Task Login_WhenRedisIsUnavailable_FailsClosedWith429()
    {
        using var factory = Configure(redis: UnreachableRedis, enabled: true);
        var client = factory.Build();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "admin@subul.iq", password = "whatever" });

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        Assert.True(response.Headers.Contains("Retry-After"));
    }

    // The 429 must be indistinguishable from a real rate-limit hit, so an
    // anonymous caller cannot use it to detect that Redis is down.
    [Fact]
    public async Task Login_WhenRedisIsUnavailable_DoesNotRevealTheOutage()
    {
        using var factory = Configure(redis: UnreachableRedis, enabled: true);
        var client = factory.Build();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "admin@subul.iq", password = "whatever" });
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Too many requests", body, StringComparison.Ordinal);
        foreach (var leak in new[] { "Redis", "redis", "unavailable", "limiter" })
            Assert.DoesNotContain(leak, body, StringComparison.Ordinal);
    }

    // A Redis outage must not take the storefront down with it: everything that
    // is not login keeps failing open, on purpose.
    [Fact]
    public async Task Catalog_WhenRedisIsUnavailable_StillFailsOpen()
    {
        using var factory = Configure(redis: UnreachableRedis, enabled: true);
        var client = factory.Build();

        var response = await client.GetAsync("/api/products?limit=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Builds a host with rate-limit settings overridden per test. Boot is
    /// deferred to <see cref="Build"/> so the startup-validation test can assert
    /// on the exception instead of failing during construction.
    /// </summary>
    private sealed class WebApplicationFactoryOf(string database, string? redis, bool enabled) : IDisposable
    {
        private TestWebApplicationFactory? _factory;
        private HttpClient? _client;

        public HttpClient Build()
        {
            _factory = new TestWebApplicationFactory(database);
            var configured = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("RedisRateLimit:Enabled", enabled ? "true" : "false");
                if (redis is not null)
                    builder.UseSetting("ConnectionStrings:Redis", redis);
            });

            _client = configured.CreateClient();
            return _client;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
    }
}
