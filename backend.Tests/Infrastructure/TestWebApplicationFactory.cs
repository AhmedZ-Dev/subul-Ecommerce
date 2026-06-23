using backend.Common.Storage;
using backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests.Infrastructure;

public class TestWebApplicationFactory(string connectionString)
    : WebApplicationFactory<Program>
{
    private readonly string _testImageRoot = Path.Combine(
        Path.GetTempPath(),
        "subul-test-img",
        Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // UseSetting runs at host level before WebApplication.CreateBuilder reads config
        builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);

        Directory.CreateDirectory(_testImageRoot);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            var imageStorageDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IImageStorageService));

            if (imageStorageDescriptor is not null)
                services.Remove(imageStorageDescriptor);

            services.AddSingleton<IImageStorageService>(
                new TestImageStorageService(_testImageRoot));
        });

        builder.UseEnvironment("Testing");
    }
}
