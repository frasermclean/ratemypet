using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RateMyPet.Database;

namespace RateMyPet.Initializer;

public class DatabaseInitializer(ApplicationDbContext dbContext, ILogger<DatabaseInitializer> logger)
{
    private static readonly ActivitySource ActivitySource = new(nameof(DatabaseInitializer));

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity(nameof(InitializeAsync), ActivityKind.Client);

        try
        {
            await EnsureDatabaseExistsAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.AddException(exception);
        }
    }

    private async Task EnsureDatabaseExistsAsync(CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            if (await dbCreator.ExistsAsync(cancellationToken))
            {
                logger.LogInformation("Database already exists");
                return;
            }

            logger.LogInformation("Creating database");
            await dbCreator.CreateAsync(cancellationToken);
        });
    }

}
