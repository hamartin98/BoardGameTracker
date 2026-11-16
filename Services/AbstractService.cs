using BoardGameTracker.Database;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class AbstractService<EntityType>
    where EntityType : AbstractEntity
{
    private readonly BoardGameContext Context;

    public AbstractService(BoardGameContext context)
    {
        Context = context;
    }

    protected async Task<EntityType> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        EntityType result =
            await Context.Set<EntityType>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException<EntityType>();
        return result;
    }

    protected async Task DeleteEntityAsync(
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

    protected async Task UpdateEntityAsync(
        EntityType entity,
        EntityType model,
        int version,
        CancellationToken cancellationToken
    )
    {
        if (entity.Version != version)
        {
            throw new VersionMismatchException();
        }
        model.Id = entity.Id;
        Context.Entry(entity).CurrentValues.SetValues(model);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
