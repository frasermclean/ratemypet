using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RateMyPet.Core;

namespace RateMyPet.Database.Interceptors;

public class UserInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
        {
            return new ValueTask<InterceptionResult<int>>(result);
        }

        foreach (var userEntry in dbContext.ChangeTracker.Entries<User>()
                     .Where(userEntry => userEntry is { State: EntityState.Added or EntityState.Modified }))
        {
            userEntry.Property(user => user.LastActivity).CurrentValue = DateTime.UtcNow;

            if (userEntry.State == EntityState.Added)
            {
                userEntry.Entity.Activities.Add(UserActivity.Register(userEntry.Entity));
            }
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }
}
