namespace RateMyPet.Initializer;

public class WorkerService(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        var storageInitializer = scope.ServiceProvider.GetRequiredService<StorageInitializer>();

        await Task.WhenAll(
            databaseInitializer.InitializeAsync(cancellationToken),
            storageInitializer.InitializeAsync(cancellationToken));

        applicationLifetime.StopApplication();
    }
}
