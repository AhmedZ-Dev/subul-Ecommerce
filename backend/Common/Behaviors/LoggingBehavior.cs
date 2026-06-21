using System.Diagnostics;
using MediatR;

namespace backend.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", name);
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();
            logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", name, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Error handling {RequestName} after {ElapsedMs}ms", name, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
