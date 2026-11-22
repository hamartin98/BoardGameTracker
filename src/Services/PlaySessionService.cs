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
        foreach (Player player in await FindPlayersAsync(model, cancellationToken))
        {
            model.Players.Add(player);
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
        List<Player> players = await FindPlayersAsync(model, cancellationToken);
        HashSet<Guid> playerIds = [.. players.Select(e => e.Id)];
        if (existing.PlayerScores.Any(e => !playerIds.Contains(e.PlayerId)))
        {
            throw new BusinessRuleException(
                "A player with a score in the play session cannot be removed."
            );
        }

        bool changed =
            ReplaceCollection(existing.UsedExpansions, expansions)
            | ReplaceCollection(existing.Players, players);
        if (changed)
        {
            // A change to the collections alone must still bump the version
            Context.Entry(existing).State = EntityState.Modified;
        }
    }

    /// <summary>
    /// Replaces the items of the collection when its ids differ; returns whether it changed
    /// </summary>
    private static bool ReplaceCollection<T>(ICollection<T> existing, List<T> replacement)
        where T : AbstractEntity
    {
        if (existing.Select(e => e.Id).ToHashSet().SetEquals(replacement.Select(e => e.Id)))
        {
            return false;
        }

        existing.Clear();
        foreach (T item in replacement)
        {
            existing.Add(item);
        }
        return true;
    }

    protected override IQueryable<PlaySession> IncludeListRelations(IQueryable<PlaySession> query)
    {
        return query
            .Include(e => e.UsedExpansions)
            .Include(e => e.Players)
            .Include(e => e.PlayerScores);
    }

    protected override async Task LoadRelations(
        PlaySession entity,
        CancellationToken cancellationToken
    )
    {
        var entry = Context.Entry(entity);
        await entry.Collection(e => e.UsedExpansions).LoadAsync(cancellationToken);
        await entry.Collection(e => e.Players).LoadAsync(cancellationToken);
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

    private async Task<List<Player>> FindPlayersAsync(
        PlaySession model,
        CancellationToken cancellationToken
    )
    {
        List<Guid> ids = [.. model.PlayerIds.Distinct()];
        List<Player> players = await Context
            .Set<Player>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (players.Count != ids.Count)
        {
            throw new EntityNotFoundException<Player>();
        }
        if (model.StartPlayerId is Guid startPlayerId && !ids.Contains(startPlayerId))
        {
            throw new BusinessRuleException(
                "The start player must be one of the session's players."
            );
        }
        return players;
    }
}
