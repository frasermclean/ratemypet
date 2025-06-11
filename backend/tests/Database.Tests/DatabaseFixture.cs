using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RateMyPet.Database.Tests;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DatabaseFixture))]

namespace RateMyPet.Database.Tests;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private IHost? host;

    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public IServiceProvider ServiceProvider =>
        host?.Services ?? throw new InvalidOperationException("Host is not initialized");

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();
        var hostBuilder = Host.CreateApplicationBuilder();

        hostBuilder.Logging.ClearProviders();

        hostBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            {
                "ConnectionStrings:database", container.GetConnectionString()
            }
        });

        host = hostBuilder.AddDatabaseServices()
            .Build();

        await InitializeDatabaseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await container.DisposeAsync();
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
