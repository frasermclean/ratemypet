using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RateMyPet.Core;

namespace RateMyPet.Database.Interceptors;

public class UserInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAddedOrModifiedUsers(eventData);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAddedOrModifiedUsers(eventData);
        return ValueTask.FromResult(result);
    }

    private void UpdateAddedOrModifiedUsers(DbContextEventData eventData)
    {
        Debug.Assert(eventData.Context is not null);

        foreach (var userEntry in eventData.Context.ChangeTracker.Entries<User>()
                     .Where(userEntry => userEntry is { State: EntityState.Added or EntityState.Modified }))
        {
            userEntry.Property(user => user.LastActivity).CurrentValue = timeProvider.GetUtcNow().DateTime;

            if (userEntry.State == EntityState.Added)
            {
                userEntry.Entity.Activities.Add(UserActivity.Register(userEntry.Entity.Id));
                continue;
            }

            if (userEntry.Property(user => user.EmailConfirmed) is { IsModified: true, CurrentValue: true })
            {
                userEntry.Entity.Activities.Add(UserActivity.ConfirmEmail(userEntry.Entity.Id));
            }
        }
    }
}
