using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardGameTracker.Database.Interceptors;

public sealed class UpdateInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
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

        IEnumerable<EntityEntry<AbstractEntity>> entries = eventData
            .Context.ChangeTracker.Entries<AbstractEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (EntityEntry<AbstractEntity> toUpdate in entries)
        {
            toUpdate.State = EntityState.Modified;
            toUpdate.Entity.Version++;
            toUpdate.Entity.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
