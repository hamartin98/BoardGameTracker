using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Statistics;

namespace BoardGameTracker.Services.Statistics;

public static class PlayerStatisticsCalculator
{
    /// <summary>
    /// Statistics of a player from the sessions they took part in; wins and bests only use competitive sessions
    /// </summary>
    public static PlayerStatistics Calculate(Guid playerId, IEnumerable<SessionRecord> sessions)
    {
        List<SessionRecord> all = [.. sessions.Where(e => e.PlayerIds.Contains(playerId))];
        List<(SessionRecord Session, ScoreRecord Score)> competitiveScores =
        [
            .. all.Where(e => e.SessionType == PlaySessionType.COMPETITIVE)
                .SelectMany(session =>
                    session
                        .Scores.Where(score => score.PlayerId == playerId)
                        .Select(score => (session, score))
                ),
        ];

        return new PlayerStatistics
        {
            PlayCount = all.Count,
            LastPlayedAt = all.Count == 0 ? null : all.Max(e => e.PlayedAt),
            TotalPlayTime = StatisticsMath.TotalPlayTime(all),
            CompetitivePlayCount = competitiveScores.Count,
            WinCount = competitiveScores.Count(e => e.Score.Win),
            WinRate = StatisticsMath.Rate(competitiveScores, e => e.Score.Win),
            MostPlayedGame = MostPlayedGame(all),
            PersonalBests = PersonalBests(competitiveScores),
        };
    }

    private static MostPlayedGame? MostPlayedGame(List<SessionRecord> sessions)
    {
        return sessions
            .GroupBy(e => e.BoardGameId)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => group.Max(e => e.PlayedAt))
            .Select(group => new MostPlayedGame
            {
                BoardGameId = group.Key,
                BoardGameName = group.First().BoardGameName,
                PlayCount = group.Count(),
            })
            .FirstOrDefault();
    }

    private static List<PersonalBest> PersonalBests(
        List<(SessionRecord Session, ScoreRecord Score)> scores
    )
    {
        return
        [
            .. scores
                .Where(e => e.Score.Score is not null)
                .GroupBy(e => e.Session.BoardGameId)
                .Select(group =>
                    group
                        .OrderByDescending(e => e.Score.Score)
                        .ThenBy(e => e.Session.PlayedAt)
                        .Select(e => new PersonalBest
                        {
                            BoardGameId = e.Session.BoardGameId,
                            BoardGameName = e.Session.BoardGameName,
                            Score = e.Score.Score!.Value,
                            SessionId = e.Session.Id,
                            PlayedAt = e.Session.PlayedAt,
                        })
                        .First()
                )
                .OrderBy(e => e.BoardGameName)
                .ThenBy(e => e.BoardGameId),
        ];
    }
}
