using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Recommendations;
using BoardGameTracker.Services.Recommendations;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class RecommendationService : IRecommendationService
{
    private readonly BoardGameContext Context;
    private readonly TimeProvider TimeProvider;
    private readonly RecommendationOptions Options;

    public RecommendationService(
        BoardGameContext context,
        TimeProvider timeProvider,
        RecommendationOptions options
    )
    {
        Context = context;
        TimeProvider = timeProvider;
        Options = options;
    }

    public async Task<List<BoardGameRecommendation>> GetRecommendationsAsync(
        RecommendationRequest request,
        CancellationToken cancellationToken
    )
    {
        byte playerCount = request.PlayerCount;
        List<BoardGame> boardGames = await Context
            .Set<BoardGame>()
            .AsNoTracking()
            .Where(e =>
                e.Type == BoardGameType.BASE_GAME
                && e.MinPlayerCount <= playerCount
                && e.MaxPlayerCount >= playerCount
            )
            .ToListAsync(cancellationToken);

        Dictionary<Guid, SessionStatistics> statistics = await GetSessionStatisticsAsync(
            [.. boardGames.Select(e => e.Id)],
            playerCount,
            cancellationToken
        );
        DateTime now = TimeProvider.GetUtcNow().UtcDateTime;
        int limit = Math.Clamp(request.Limit ?? Options.DefaultLimit, 1, Options.MaxLimit);

        return
        [
            .. boardGames
                .Select(e =>
                    RecommendationScorer.Score(
                        e,
                        statistics.GetValueOrDefault(e.Id, SessionStatistics.NeverPlayed),
                        request,
                        Options,
                        now
                    )
                )
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.LastPlayedAt ?? DateTime.MinValue)
                .ThenBy(e => e.BoardGame.Name)
                .ThenBy(e => e.BoardGame.Id)
                .Take(limit),
        ];
    }

    /// <summary>
    /// Aggregates the sessions in memory, so no provider-specific TimeSpan average is needed
    /// </summary>
    private async Task<Dictionary<Guid, SessionStatistics>> GetSessionStatisticsAsync(
        List<Guid> boardGameIds,
        byte playerCount,
        CancellationToken cancellationToken
    )
    {
        var sessions = await Context
            .Set<PlaySession>()
            .AsNoTracking()
            .Where(e => boardGameIds.Contains(e.BoardGameId))
            .Select(e => new
            {
                e.BoardGameId,
                PlayedAt = e.StartTime ?? e.CreatedAt,
                e.Duration,
                PlayerCount = e.Players.Count,
            })
            .ToListAsync(cancellationToken);

        return sessions
            .GroupBy(e => e.BoardGameId)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    List<TimeSpan> durations =
                    [
                        .. group
                            .Where(e => e.Duration is not null && e.PlayerCount == playerCount)
                            .Select(e => e.Duration!.Value),
                    ];
                    TimeSpan? average =
                        durations.Count == 0
                            ? null
                            : TimeSpan.FromTicks((long)durations.Average(e => e.Ticks));
                    return new SessionStatistics(
                        group.Max(e => e.PlayedAt),
                        average,
                        group.Count()
                    );
                }
            );
    }
}
