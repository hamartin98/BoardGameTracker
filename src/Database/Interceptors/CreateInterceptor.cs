using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardGameTracker.Database.Interceptors;

/// <summary>
/// Sets CreatedAt and UpdatedAt of added entities to the current time, unless they are already set
/// </summary>
public sealed class CreateInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        DateTime now = timeProvider.GetUtcNow().UtcDateTime;
        foreach (
            var added in eventData
                .Context.ChangeTracker.Entries<AbstractEntity>()
                .Where(e => e.State == EntityState.Added)
        )
        {
            if (added.Entity.CreatedAt == default)
            {
                added.Entity.CreatedAt = now;
            }
            if (added.Entity.UpdatedAt == default)
            {
                added.Entity.UpdatedAt = added.Entity.CreatedAt;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
