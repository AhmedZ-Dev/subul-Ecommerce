using backend.Common.Middleware;
using backend.Common.Storage;
using backend.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var maxImageBytes = builder.Configuration
    .GetSection(ImageStorageOptions.SectionName)
    .GetValue<long>("MaxFileSizeBytes", 5_242_880);
var multipartLimit = maxImageBytes + 65_536;

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = multipartLimit;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = multipartLimit;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (corsOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

if (corsOrigins.Length > 0)
{
    app.UseCors();
}

var imgPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "img"));
Directory.CreateDirectory(imgPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imgPath),
    RequestPath = "/img"
});

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
