using backend.Common.Auth;
using backend.Common.Behaviors;
using backend.Common.RateLimiting;
using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace backend.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var redisConnectionString = configuration.GetConnectionString("Redis");

        services.AddOptions<RedisRateLimitOptions>()
            .Bind(configuration.GetSection(RedisRateLimitOptions.SectionName))
            .Validate(options => options.PermitLimit > 0, "RedisRateLimit:PermitLimit must be greater than zero.")
            .Validate(options => options.WindowSeconds > 0, "RedisRateLimit:WindowSeconds must be greater than zero.")
            .Validate(options => options.LoginPermitLimit > 0, "RedisRateLimit:LoginPermitLimit must be greater than zero.")
            .Validate(options => options.LoginWindowSeconds > 0, "RedisRateLimit:LoginWindowSeconds must be greater than zero.")
            .ValidateOnStart();

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddSingleton<RedisRateLimiter>();

            services.AddStackExchangeRedisOutputCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "subul:";
            });
        }

        services.AddOutputCache(options =>
        {
            if (!string.IsNullOrWhiteSpace(redisConnectionString))
            {
                options.AddBasePolicy(policy => policy
                    .With(context => context.HttpContext.Request.Path.StartsWithSegments("/api/categories"))
                    .Expire(TimeSpan.FromSeconds(30))
                    .Tag("categories")
                    .Cache());
            }
        });

        services.Configure<ImageStorageOptions>(configuration.GetSection(ImageStorageOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<JwtTokenService>();
        services.AddSingleton<IImageStorageService, LocalImageStorageService>();

        return services;
    }
}
