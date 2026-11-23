using System.Linq.Expressions;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

/// <summary>
/// CRUD service whose create and update take their own input types, holding the entity's values
/// </summary>
public abstract class AbstractService<EntityType, FilterType, CreateType, UpdateType>
    : ICrudService<EntityType, FilterType, CreateType, UpdateType>
    where EntityType : AbstractEntity
    where FilterType : AbstractFilter<EntityType>
    where CreateType : class
    where UpdateType : class
{
    protected readonly BoardGameContext Context;

    protected AbstractService(BoardGameContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Entity to store from a create input
    /// </summary>
    protected abstract EntityType GetCreateValues(CreateType model);

    /// <summary>
    /// Values to apply to the stored entity from an update input
    /// </summary>
    protected abstract EntityType GetUpdateValues(UpdateType model);

    public virtual async Task<EntityType> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        EntityType result = await FindByIdAsync(id, cancellationToken);
        await LoadRelations(result, cancellationToken);
        return result;
    }

    public virtual async Task<EntityType> CreateAsync(
        CreateType model,
        CancellationToken cancellationToken
    )
    {
        await ValidateCreateAsync(model, cancellationToken);
        var entity = await Context
            .Set<EntityType>()
            .AddAsync(GetCreateValues(model), cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity.Entity;
    }

    public virtual async Task<EntityType> UpdateAsync(
        UpdateType model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        EntityType entity = await FindByIdAsync(id, cancellationToken);
        ThrowIfVersionMismatch(entity, version);

        await ValidateUpdateAsync(entity, model, cancellationToken);

        // Keep stored metadata; the interceptors update Version and UpdatedAt on save
        EntityType values = GetUpdateValues(model);
        values.Id = entity.Id;
        values.CreatedAt = entity.CreatedAt;
        values.UpdatedAt = entity.UpdatedAt;
        values.Version = entity.Version;
        values.IsDeleted = entity.IsDeleted;

        Context.Entry(entity).CurrentValues.SetValues(values);
        await UpdateRelationsAsync(entity, model, cancellationToken);
        await SaveVersionedChangesAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<PaginatedResponse<EntityType>> GetAllAsync(
        FilterType filter,
        CancellationToken cancellationToken
    )
    {
        var query = IncludeListRelations(Context.Set<EntityType>().AsNoTracking());
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

        ThrowIfVersionMismatch(entity, version);

        await LoadDeleteRelationsAsync(entity, cancellationToken);
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
    protected async Task SaveVersionedChangesAsync(CancellationToken cancellationToken)
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

    protected static void ThrowIfVersionMismatch(EntityType entity, int version)
    {
        if (entity.Version != version)
        {
            throw new VersionMismatchException();
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
    /// Runs before a new entity is saved
    /// </summary>
    protected virtual Task ValidateCreateAsync(
        CreateType model,
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Runs after the stored entity is loaded and its version checked, before the update is applied
    /// </summary>
    protected virtual Task ValidateUpdateAsync(
        EntityType existing,
        UpdateType model,
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Runs after the scalar values of an update are applied, before saving; for collection navigations
    /// </summary>
    protected virtual Task UpdateRelationsAsync(
        EntityType existing,
        UpdateType model,
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Includes the navigations needed by list responses
    /// </summary>
    protected virtual IQueryable<EntityType> IncludeListRelations(IQueryable<EntityType> query)
    {
        return query;
    }

    /// <summary>
    /// Loads the navigations needed by get-by-id responses
    /// </summary>
    protected virtual Task LoadRelations(EntityType entity, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads the dependents that a delete should soft-delete too; only loaded dependents are cascaded
    /// </summary>
    protected virtual Task LoadDeleteRelationsAsync(
        EntityType entity,
        CancellationToken cancellationToken
    )
    {
        return LoadRelations(entity, cancellationToken);
    }
}

/// <summary>
/// CRUD service whose create and update take the entity itself
/// </summary>
public abstract class AbstractService<EntityType, FilterType>
    : AbstractService<EntityType, FilterType, EntityType, EntityType>,
        ICrudService<EntityType, FilterType>
    where EntityType : AbstractEntity
    where FilterType : AbstractFilter<EntityType>
{
    protected AbstractService(BoardGameContext context)
        : base(context) { }

    protected override EntityType GetCreateValues(EntityType model)
    {
        return model;
    }

    protected override EntityType GetUpdateValues(EntityType model)
    {
        return model;
    }
}
