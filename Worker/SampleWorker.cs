using Microsoft.Extensions.Hosting;

namespace SampleProject.Worker;

/// <summary>
/// Minimal background worker demonstrating registration and periodic work.
/// </summary>
public class SampleWorker : BackgroundService
{
    /// <summary>
    /// Runs the background loop until cancellation is requested.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Do background work (e.g., cleanup or messaging)
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
