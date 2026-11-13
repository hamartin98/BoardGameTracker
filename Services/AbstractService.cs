using BoardGameTracker.Database;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Services;

public class AbstractService<EntityType>
    where EntityType : AbstractEntity
{
    private readonly BoardGameContext Context;

    public AbstractService(BoardGameContext context)
    {
        Context = context;
    }

    protected async Task DeleteEntity(
        EntityType entity,
        int version,
        CancellationToken cancellationToken
    )
    {
        if (entity.Version != version)
        {
            throw new VersionMismatchException();
        }
        Context.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
