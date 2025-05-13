using Microsoft.EntityFrameworkCore;
using RateMyPet.Database;

namespace RateMyPet.Initializer;

public class WorkerService(
    ILogger<WorkerService> logger,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime applicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeDatabaseAsync(stoppingToken);

        applicationLifetime.StopApplication();
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);

        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pendingMigrations.Count > 0)
        {
            logger.LogInformation("Attempting to apply {PendingMigrationsCount} pending migrations",
                pendingMigrations.Count);
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            logger.LogInformation("Database is up to date with all migrations");
        }
    }
}
