using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using RateMyPet.Core;
using RateMyPet.Database;

namespace RateMyPet.Initializer;

public class DatabaseInitializer(ApplicationDbContext dbContext, Tracer tracer)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var span = tracer.StartActiveSpan("Initialize database");

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            span.RecordException(exception);
            throw;
        }
    }

    public static async Task SeedAsync(DbContext dbContext, bool _, CancellationToken cancellationToken = default)
    {
        foreach (var user in SeedData.Users.Values)
        {
            if (await dbContext.Set<User>().AnyAsync(u => u.Id == user.Id, cancellationToken))
            {
                continue;
            }

            dbContext.Add(user);
        }

        if (!await dbContext.Set<Post>().AnyAsync(cancellationToken))
        {
            dbContext.AddRange(SeedData.Posts);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
