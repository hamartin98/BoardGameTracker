using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class PlaySessionService
    : AbstractService<PlaySession, PlaySessionFilter>,
        IPlaySessionService
{
    private readonly TimeProvider TimeProvider;

    public PlaySessionService(BoardGameContext context, TimeProvider timeProvider)
        : base(context)
    {
        TimeProvider = timeProvider;
    }

    public Task<PlaySession> StartAsync(PlaySession model, CancellationToken cancellationToken)
    {
        model.StartTime = TimeProvider.GetUtcNow().UtcDateTime;
        model.Duration = null;
        return CreateAsync(model, cancellationToken);
    }

    public async Task<PlaySession> FinishAsync(
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        PlaySession session = await FindByIdAsync(id, cancellationToken);
        ThrowIfVersionMismatch(session, version);

        if (session.Duration is not null)
        {
            throw new BusinessRuleException("The play session is already finished.");
        }
        if (session.StartTime is null)
        {
            throw new BusinessRuleException("The play session has no start time.");
        }
        TimeSpan duration = TimeProvider.GetUtcNow().UtcDateTime - session.StartTime.Value;
        if (duration <= TimeSpan.Zero)
        {
            throw new BusinessRuleException("The play session has not started yet.");
        }

        session.Duration = duration;
        await SaveVersionedChangesAsync(cancellationToken);
        await LoadRelations(session, cancellationToken);
        return session;
    }

    protected override async Task ValidateCreateAsync(
        PlaySession model,
        CancellationToken cancellationToken
    )
    {
        await CheckBoardGameAsync(model.BoardGameId, cancellationToken);
        foreach (BoardGame expansion in await FindExpansionsAsync(model, cancellationToken))
        {
            model.UsedExpansions.Add(expansion);
        }
    }

    protected override Task ValidateUpdateAsync(
        PlaySession existing,
        PlaySession model,
        CancellationToken cancellationToken
    )
    {
        return CheckBoardGameAsync(model.BoardGameId, cancellationToken);
    }

    protected override async Task UpdateRelationsAsync(
        PlaySession existing,
        PlaySession model,
        CancellationToken cancellationToken
    )
    {
        await LoadRelations(existing, cancellationToken);
        List<BoardGame> expansions = await FindExpansionsAsync(model, cancellationToken);

        bool changed = !existing
            .UsedExpansions.Select(e => e.Id)
            .ToHashSet()
            .SetEquals(expansions.Select(e => e.Id));
        if (!changed)
        {
            return;
        }

        existing.UsedExpansions.Clear();
        foreach (BoardGame expansion in expansions)
        {
            existing.UsedExpansions.Add(expansion);
        }
        // A change to the expansions alone must still bump the version
        Context.Entry(existing).State = EntityState.Modified;
    }

    protected override IQueryable<PlaySession> IncludeListRelations(IQueryable<PlaySession> query)
    {
        return query.Include(e => e.UsedExpansions).Include(e => e.PlayerScores);
    }

    protected override async Task LoadRelations(
        PlaySession entity,
        CancellationToken cancellationToken
    )
    {
        var entry = Context.Entry(entity);
        await entry.Collection(e => e.UsedExpansions).LoadAsync(cancellationToken);
        await entry.Collection(e => e.PlayerScores).LoadAsync(cancellationToken);
    }

    public async Task<List<PlayerScore>> GetPlayerScoresAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<PlaySession>(id, cancellationToken);
        return await Context
            .Set<PlayerScore>()
            .Where(e => e.SessionId == id)
            .OrderBy(e => e.CreatedAt)
            .ThenBy(e => e.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private async Task CheckBoardGameAsync(Guid boardGameId, CancellationToken cancellationToken)
    {
        BoardGame boardGame =
            await Context
                .Set<BoardGame>()
                .FirstOrDefaultAsync(e => e.Id == boardGameId, cancellationToken)
            ?? throw new EntityNotFoundException<BoardGame>();
        if (boardGame.Type != BoardGameType.BASE_GAME)
        {
            throw new BusinessRuleException("A play session must be for a base game.");
        }
    }

    private async Task<List<BoardGame>> FindExpansionsAsync(
        PlaySession model,
        CancellationToken cancellationToken
    )
    {
        List<Guid> ids = [.. model.UsedExpansionIds.Distinct()];
        List<BoardGame> expansions = await Context
            .Set<BoardGame>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (expansions.Count != ids.Count)
        {
            throw new EntityNotFoundException<BoardGame>();
        }
        if (
            expansions.Any(e =>
                e.Type != BoardGameType.EXPANSION || e.ExpansionOfId != model.BoardGameId
            )
        )
        {
            throw new BusinessRuleException(
                "Used expansions must be expansions of the played board game."
            );
        }
        return expansions;
    }
}
