using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class ScoringCategoryService
    : AbstractService<ScoringCategory, ScoringCategoryFilter>,
        IScoringCategoryService
{
    public ScoringCategoryService(BoardGameContext context)
        : base(context) { }

    protected override async Task ValidateCreateAsync(
        ScoringCategory model,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<BoardGame>(model.BoardGameId, cancellationToken);
        await ThrowIfNameTakenAsync(model.BoardGameId, model.Name, null, cancellationToken);
        model.Order = await Context
            .Set<ScoringCategory>()
            .CountAsync(e => e.BoardGameId == model.BoardGameId, cancellationToken);
    }

    protected override Task ValidateUpdateAsync(
        ScoringCategory existing,
        ScoringCategory model,
        CancellationToken cancellationToken
    )
    {
        model.BoardGameId = existing.BoardGameId;
        model.Order = existing.Order;
        return ThrowIfNameTakenAsync(
            existing.BoardGameId,
            model.Name,
            existing.Id,
            cancellationToken
        );
    }

    public override async Task DeleteAsync(
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        ScoringCategory entity = await FindByIdAsync(id, cancellationToken);
        ThrowIfVersionMismatch(entity, version);

        if (
            await Context
                .Set<CategoryScore>()
                .AnyAsync(e => e.ScoringCategoryId == id, cancellationToken)
        )
        {
            throw new BusinessRuleException(
                "A scoring category with recorded scores cannot be deleted."
            );
        }

        // Keep the remaining positions contiguous
        List<ScoringCategory> later = await Context
            .Set<ScoringCategory>()
            .Where(e => e.BoardGameId == entity.BoardGameId && e.Order > entity.Order)
            .ToListAsync(cancellationToken);
        foreach (ScoringCategory category in later)
        {
            category.Order--;
        }

        Context.Remove(entity);
        await SaveVersionedChangesAsync(cancellationToken);
    }

    public async Task<List<ScoringCategory>> GetByBoardGameAsync(
        Guid boardGameId,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<BoardGame>(boardGameId, cancellationToken);
        return await Context
            .Set<ScoringCategory>()
            .Where(e => e.BoardGameId == boardGameId)
            .OrderBy(e => e.Order)
            .ThenBy(e => e.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ScoringCategory>> ReorderAsync(
        Guid boardGameId,
        List<Guid> orderedIds,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<BoardGame>(boardGameId, cancellationToken);
        List<ScoringCategory> categories = await Context
            .Set<ScoringCategory>()
            .Where(e => e.BoardGameId == boardGameId)
            .ToListAsync(cancellationToken);

        if (
            orderedIds.Count != categories.Count
            || !categories.Select(e => e.Id).ToHashSet().SetEquals(orderedIds)
        )
        {
            throw new BusinessRuleException(
                "The order must list every scoring category of the board game exactly once."
            );
        }

        foreach (ScoringCategory category in categories)
        {
            category.Order = orderedIds.IndexOf(category.Id);
        }
        await SaveVersionedChangesAsync(cancellationToken);
        return [.. categories.OrderBy(e => e.Order)];
    }

    private Task ThrowIfNameTakenAsync(
        Guid boardGameId,
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken
    )
    {
        string normalizedName = name.ToLowerInvariant();
        return ThrowIfDuplicateAsync(
            e => e.BoardGameId == boardGameId && e.NormalizedName == normalizedName,
            excludedId,
            "A scoring category with this name already exists for the board game.",
            cancellationToken
        );
    }
}
