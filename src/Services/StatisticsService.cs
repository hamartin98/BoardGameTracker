using System.Linq.Expressions;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Services.Statistics;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class StatisticsService : IStatisticsService
{
    private readonly BoardGameContext Context;

    public StatisticsService(BoardGameContext context)
    {
        Context = context;
    }

    public async Task LoadBoardGameStatisticsAsync(
        IReadOnlyCollection<BoardGame> boardGames,
        CancellationToken cancellationToken
    )
    {
        if (boardGames.Count == 0)
        {
            return;
        }
        List<Guid> ids = [.. boardGames.Select(e => e.Id)];
        List<SessionRecord> sessions = await GetSessionsAsync(
            e => ids.Contains(e.BoardGameId) || e.UsedExpansions.Any(x => ids.Contains(x.Id)),
            cancellationToken
        );
        foreach (BoardGame boardGame in boardGames)
        {
            boardGame.Statistics = BoardGameStatisticsCalculator.Calculate(
                sessions.Where(e =>
                    e.BoardGameId == boardGame.Id || e.UsedExpansionIds.Contains(boardGame.Id)
                )
            );
        }
    }

    public async Task LoadPlayerStatisticsAsync(
        IReadOnlyCollection<Player> players,
        CancellationToken cancellationToken
    )
    {
        if (players.Count == 0)
        {
            return;
        }
        List<Guid> ids = [.. players.Select(e => e.Id)];
        List<SessionRecord> sessions = await GetSessionsAsync(
            e => e.Players.Any(p => ids.Contains(p.Id)),
            cancellationToken
        );
        foreach (Player player in players)
        {
            player.Statistics = PlayerStatisticsCalculator.Calculate(player.Id, sessions);
        }
    }

    /// <summary>
    /// Loads the matching sessions in one query and aggregates them in memory, so no provider-specific TimeSpan math is needed
    /// </summary>
    private Task<List<SessionRecord>> GetSessionsAsync(
        Expression<Func<PlaySession, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return Context
            .Set<PlaySession>()
            .AsNoTracking()
            .Where(predicate)
            .Select(e => new SessionRecord(
                e.Id,
                e.BoardGameId,
                e.BoardGame!.Name,
                e.UsedExpansions.Select(x => x.Id).ToList(),
                e.SessionType,
                e.StartTime ?? e.CreatedAt,
                e.Duration,
                e.Players.Select(p => p.Id).ToList(),
                e.StartPlayerId,
                e.PlayerScores.Select(s => new ScoreRecord(
                        s.PlayerId,
                        s.Player!.Name,
                        s.Score,
                        s.Win
                    ))
                    .ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}
