using Microsoft.EntityFrameworkCore;
using RateMyPet.Database;

namespace RateMyPet.Initializer;

public class WorkerService(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        await databaseInitializer.InitializeAsync(cancellationToken);

        applicationLifetime.StopApplication();
    }
}
