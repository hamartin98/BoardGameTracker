using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class PlayerScoreService
    : AbstractService<PlayerScore, PlayerScoreFilter>,
        IPlayerScoreService
{
    public PlayerScoreService(BoardGameContext context)
        : base(context) { }

    protected override async Task ValidateCreateAsync(
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<Player>(model.PlayerId, cancellationToken);
        await ThrowIfNotExistsAsync<PlaySession>(model.SessionId, cancellationToken);

        Guid playerId = model.PlayerId;
        Guid sessionId = model.SessionId;
        bool isSessionPlayer = await Context
            .Set<PlaySession>()
            .AnyAsync(
                e => e.Id == sessionId && e.Players.Any(p => p.Id == playerId),
                cancellationToken
            );
        if (!isSessionPlayer)
        {
            throw new BusinessRuleException("The player is not a player of the play session.");
        }

        await ThrowIfDuplicateAsync(
            e => e.PlayerId == playerId && e.SessionId == sessionId,
            null,
            "The player already has a score in this play session.",
            cancellationToken
        );

        model.Score = await CheckCategoryScoresAsync(model, cancellationToken);
    }

    protected override async Task ValidateUpdateAsync(
        PlayerScore existing,
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        model.PlayerId = existing.PlayerId;
        model.SessionId = existing.SessionId;
        // Without categories, a total recorded without a breakdown is kept
        model.Score = await CheckCategoryScoresAsync(model, cancellationToken) ?? existing.Score;
    }

    protected override async Task UpdateRelationsAsync(
        PlayerScore existing,
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        await LoadRelations(existing, cancellationToken);
        Dictionary<Guid, int> values = model.CategoryScores.ToDictionary(
            e => e.ScoringCategoryId,
            e => e.Value
        );

        bool changed = false;
        foreach (CategoryScore categoryScore in existing.CategoryScores.ToList())
        {
            if (!values.TryGetValue(categoryScore.ScoringCategoryId, out int value))
            {
                existing.CategoryScores.Remove(categoryScore);
                changed = true;
            }
            else if (categoryScore.Value != value)
            {
                categoryScore.Value = value;
                changed = true;
            }
        }
        HashSet<Guid> existingIds = [.. existing.CategoryScores.Select(e => e.ScoringCategoryId)];
        foreach ((Guid categoryId, int value) in values.Where(e => !existingIds.Contains(e.Key)))
        {
            existing.CategoryScores.Add(new() { ScoringCategoryId = categoryId, Value = value });
            changed = true;
        }

        if (changed)
        {
            // A change to the category scores alone must still bump the version
            Context.Entry(existing).State = EntityState.Modified;
        }
    }

    protected override IQueryable<PlayerScore> IncludeListRelations(IQueryable<PlayerScore> query)
    {
        return query.Include(e => e.CategoryScores);
    }

    protected override Task LoadRelations(PlayerScore entity, CancellationToken cancellationToken)
    {
        return Context.Entry(entity).Collection(e => e.CategoryScores).LoadAsync(cancellationToken);
    }

    /// <summary>
    /// Category scores stay stored with a soft-deleted player score, so nothing is cascaded
    /// </summary>
    protected override Task LoadDeleteRelationsAsync(
        PlayerScore entity,
        CancellationToken cancellationToken
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks that every category of the session is scored exactly once; returns the total, or null when the session has no categories
    /// </summary>
    private async Task<int?> CheckCategoryScoresAsync(
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        List<ScoringCategory> categories = await Context.GetSessionScoringCategoriesAsync(
            model.SessionId,
            cancellationToken
        );
        HashSet<Guid> sessionCategoryIds = [.. categories.Select(e => e.Id)];
        List<Guid> requestedIds = [.. model.CategoryScores.Select(e => e.ScoringCategoryId)];

        List<Guid> foreignIds =
        [
            .. requestedIds.Where(e => !sessionCategoryIds.Contains(e)).Distinct(),
        ];
        if (foreignIds.Count != 0)
        {
            int found = await Context
                .Set<ScoringCategory>()
                .CountAsync(e => foreignIds.Contains(e.Id), cancellationToken);
            if (found != foreignIds.Count)
            {
                throw new EntityNotFoundException<ScoringCategory>();
            }
            throw new BusinessRuleException(
                "Scoring categories must belong to the session's board game or its used expansions."
            );
        }
        if (
            requestedIds.Count != sessionCategoryIds.Count
            || !sessionCategoryIds.SetEquals(requestedIds)
        )
        {
            throw new BusinessRuleException(
                "Every scoring category of the play session must be scored exactly once."
            );
        }

        return categories.Count == 0 ? null : model.CategoryScores.Sum(e => e.Value);
    }
}
