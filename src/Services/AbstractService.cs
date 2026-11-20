using System.Linq.Expressions;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public abstract class AbstractService<EntityType, FilterType> : ICrudService<EntityType, FilterType>
    where EntityType : AbstractEntity
    where FilterType : AbstractFilter<EntityType>
{
    protected readonly BoardGameContext Context;

    public AbstractService(BoardGameContext context)
    {
        Context = context;
    }

    public virtual async Task<EntityType> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        EntityType result = await FindByIdAsync(id, cancellationToken);
        await LoadRelations(result, cancellationToken);
        return result;
    }

    public virtual async Task<EntityType> CreateAsync(
        EntityType model,
        CancellationToken cancellationToken
    )
    {
        var entity = await Context.Set<EntityType>().AddAsync(model, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    public virtual async Task<EntityType> UpdateAsync(
        EntityType model,
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

        // Keep stored metadata; the interceptors update Version and UpdatedAt on save
        model.Id = entity.Id;
        model.CreatedAt = entity.CreatedAt;
        model.UpdatedAt = entity.UpdatedAt;
        model.Version = entity.Version;
        model.IsDeleted = entity.IsDeleted;

        Context.Entry(entity).CurrentValues.SetValues(model);
        await SaveVersionedChangesAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<PaginatedResponse<EntityType>> GetAllAsync(
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

    public virtual async Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        EntityType entity = await FindByIdAsync(id, cancellationToken);

        if (entity.Version != version)
        {
            throw new VersionMismatchException();
        }

        await LoadRelations(entity, cancellationToken);
        Context.Remove(entity);
        await SaveVersionedChangesAsync(cancellationToken);
    }

    protected async Task<EntityType> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        EntityType result =
            await Context.Set<EntityType>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException<EntityType>();
        return result;
    }

    /// <summary>
    /// Saves an update or delete; a concurrent change to the same row becomes a VersionMismatchException
    /// </summary>
    private async Task SaveVersionedChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new VersionMismatchException(innerException: exception);
        }
    }

    protected async Task<bool> ExistsAsync<TReferenced>(
        Guid id,
        CancellationToken cancellationToken
    )
        where TReferenced : AbstractEntity
    {
        return await Context.Set<TReferenced>().FindAsync([id], cancellationToken) is not null;
    }

    /// <summary>
    /// Throws BusinessRuleException (422) when another non-deleted entity matches the predicate
    /// </summary>
    protected async Task ThrowIfDuplicateAsync(
        Expression<Func<EntityType, bool>> predicate,
        Guid? excludedId,
        string message,
        CancellationToken cancellationToken
    )
    {
        bool duplicate = await Context
            .Set<EntityType>()
            .Where(predicate)
            .AnyAsync(e => e.Id != excludedId, cancellationToken);
        if (duplicate)
        {
            throw new BusinessRuleException(message);
        }
    }

    protected async Task ThrowIfNotExistsAsync<TReferenced>(
        Guid id,
        CancellationToken cancellationToken
    )
        where TReferenced : AbstractEntity
    {
        if (!await ExistsAsync<TReferenced>(id, cancellationToken))
        {
            throw new EntityNotFoundException<TReferenced>();
        }
    }

    /// <summary>
    /// Loads the navigations needed by get-by-id responses and cascading deletes
    /// </summary>
    protected virtual Task LoadRelations(EntityType entity, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
