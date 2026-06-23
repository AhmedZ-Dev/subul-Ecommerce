using backend.Common.Middleware;
using backend.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
