using OpenTelemetry.Trace;

namespace RateMyPet.Initializer;

public class WorkerService(
    IServiceScopeFactory serviceScopeFactory,
    IHostApplicationLifetime applicationLifetime,
    Tracer tracer)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var span = tracer.StartActiveSpan("Initialize systems");

        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        var storageInitializer = scope.ServiceProvider.GetRequiredService<StorageInitializer>();

        await Task.WhenAll(
            databaseInitializer.InitializeAsync(cancellationToken),
            storageInitializer.InitializeAsync(cancellationToken));

        applicationLifetime.StopApplication();
    }
}
