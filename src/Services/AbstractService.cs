using BoardGameTracker.Database;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public abstract class AbstractService<EntityType, FilterType>
    where EntityType : AbstractEntity
    where FilterType : AbstractFilter<EntityType>
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

    protected async Task<EntityType> CreateEntityAsync(
        EntityType model,
        CancellationToken cancellationToken
    )
    {
        var entity = await Context.Set<EntityType>().AddAsync(model, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    protected async Task DeleteEntityAsync(
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        EntityType entity = await FindByIdAsync(id, cancellationToken);

        if (entity.Version != version)
        {
            throw new VersionMismatchException();
        }

        await LoadRelations(entity, cancellationToken);
        Context.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    protected async Task<EntityType> UpdateEntityAsync(
        Guid id,
        EntityType model,
        int version,
        CancellationToken cancellationToken
    )
    {
        EntityType entity = await FindByIdAsync(id, cancellationToken);
        if (entity.Version != version)
        {
            throw new VersionMismatchException();
        }
        // Keep stored metadata; the interceptors update Version and UpdatedAt on save
        model.Id = entity.Id;
        model.CreatedAt = entity.CreatedAt;
        model.UpdatedAt = entity.UpdatedAt;
        model.Version = entity.Version;
        model.IsDeleted = entity.IsDeleted;

        Context.Entry(entity).CurrentValues.SetValues(model);
        await Context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    protected async Task<PaginatedResponse<EntityType>> GetAllFilteredAsync(
        FilterType filter,
        CancellationToken cancellationToken
    )
    {
        var query = Context.Set<EntityType>().AsQueryable().AsNoTracking();
        query = filter.FilterQuery(query);
        var result = await PaginatedResponse<EntityType>.CreateAsync(
            query,
            filter.Page,
            filter.PageSize,
            cancellationToken
        );
        return result;
    }

    protected async Task<bool> Exists<Type>(Guid id, CancellationToken cancellationToken)
        where Type : AbstractEntity
    {
        return await Context
            .Set<Type>()
            .FindAsync([id, cancellationToken], cancellationToken: cancellationToken)
            is not null;
    }

    protected async Task ThrowIfNotExists<Type>(Guid id, CancellationToken cancellationToken)
        where Type : AbstractEntity
    {
        if (!await Exists<Type>(id, cancellationToken))
        {
            throw new EntityNotFoundException<Type>();
        }
    }

    protected abstract Task LoadRelations(EntityType entity, CancellationToken cancellationToken);
}
