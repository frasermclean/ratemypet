namespace RateMyPet.Initializer;

public class WorkerService(ILogger<WorkerService> logger, IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker service started");

        applicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}
