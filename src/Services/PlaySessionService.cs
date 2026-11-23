using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Commands;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class PlaySessionService
    : AbstractService<
        PlaySession,
        PlaySessionFilter,
        CreatePlaySessionCommand,
        UpdatePlaySessionCommand
    >,
        IPlaySessionService
{
    private readonly TimeProvider TimeProvider;
    private readonly Random Random;

    public PlaySessionService(BoardGameContext context, TimeProvider timeProvider, Random random)
        : base(context)
    {
        TimeProvider = timeProvider;
        Random = random;
    }

    protected override PlaySession GetCreateValues(CreatePlaySessionCommand model)
    {
        return model.Values;
    }

    protected override PlaySession GetUpdateValues(UpdatePlaySessionCommand model)
    {
        return model.Values;
    }

    public override Task<PlaySession> CreateAsync(
        CreatePlaySessionCommand model,
        CancellationToken cancellationToken
    )
    {
        if (model.StartNow)
        {
            model.Values.StartTime = TimeProvider.GetUtcNow().UtcDateTime;
            model.Values.Duration = null;
        }
        return base.CreateAsync(model, cancellationToken);
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
        CreatePlaySessionCommand model,
        CancellationToken cancellationToken
    )
    {
        PlaySession session = model.Values;
        await CheckBoardGameAsync(session.BoardGameId, cancellationToken);
        foreach (BoardGame expansion in await FindExpansionsAsync(model, cancellationToken))
        {
            session.UsedExpansions.Add(expansion);
        }
        List<Player> players = await FindPlayersAsync(model, cancellationToken);
        foreach (Player player in players)
        {
            session.Players.Add(player);
        }
        if (model.RandomStartPlayer)
        {
            if (players.Count == 0)
            {
                throw new BusinessRuleException(
                    "A random start player needs at least one player in the play session."
                );
            }
            session.StartPlayerId = players[Random.Next(players.Count)].Id;
        }
    }

    protected override Task ValidateUpdateAsync(
        PlaySession existing,
        UpdatePlaySessionCommand model,
        CancellationToken cancellationToken
    )
    {
        return CheckBoardGameAsync(model.Values.BoardGameId, cancellationToken);
    }

    protected override async Task UpdateRelationsAsync(
        PlaySession existing,
        UpdatePlaySessionCommand model,
        CancellationToken cancellationToken
    )
    {
        await LoadRelations(existing, cancellationToken);
        List<BoardGame> expansions = await FindExpansionsAsync(model, cancellationToken);
        List<Player> players = await FindPlayersAsync(model, cancellationToken);
        await ThrowIfScoredGameRemovedAsync(
            existing,
            model.Values.BoardGameId,
            expansions,
            cancellationToken
        );
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
    /// Throws when the update drops the board game or an expansion whose categories have scores in the session
    /// </summary>
    private async Task ThrowIfScoredGameRemovedAsync(
        PlaySession existing,
        Guid boardGameId,
        List<BoardGame> expansions,
        CancellationToken cancellationToken
    )
    {
        Guid sessionId = existing.Id;
        List<Guid> scoredBoardGameIds = await Context
            .Set<CategoryScore>()
            .Where(e => e.PlayerScore!.SessionId == sessionId)
            .Select(e => e.ScoringCategory!.BoardGameId)
            .Distinct()
            .ToListAsync(cancellationToken);

        HashSet<Guid> keptBoardGameIds = [boardGameId, .. expansions.Select(e => e.Id)];
        if (scoredBoardGameIds.Any(e => !keptBoardGameIds.Contains(e)))
        {
            throw new BusinessRuleException(
                "The board game or an expansion with scored categories cannot be removed from the play session."
            );
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
            .Include(e => e.CategoryScores)
            .OrderBy(e => e.CreatedAt)
            .ThenBy(e => e.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<List<ScoringCategory>> GetScoringCategoriesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Context.GetSessionScoringCategoriesAsync(id, cancellationToken);
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
        UpdatePlaySessionCommand model,
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
                e.Type != BoardGameType.EXPANSION || e.ExpansionOfId != model.Values.BoardGameId
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
        UpdatePlaySessionCommand model,
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
        if (model.Values.StartPlayerId is Guid startPlayerId && !ids.Contains(startPlayerId))
        {
            throw new BusinessRuleException(
                "The start player must be one of the session's players."
            );
        }
        return players;
    }
}
