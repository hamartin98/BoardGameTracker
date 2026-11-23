using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Statistics;

namespace BoardGameTracker.Services.Statistics;

public static class BoardGameStatisticsCalculator
{
    /// <summary>
    /// Statistics from the sessions of a board game; win rates and scores only use competitive sessions
    /// </summary>
    public static BoardGameStatistics Calculate(IEnumerable<SessionRecord> sessions)
    {
        List<SessionRecord> all = [.. sessions];
        List<SessionRecord> competitive =
        [
            .. all.Where(e => e.SessionType == PlaySessionType.COMPETITIVE),
        ];
        List<(SessionRecord Session, ScoreRecord Score)> competitiveScores =
        [
            .. competitive.SelectMany(session => session.Scores.Select(score => (session, score))),
        ];
        List<SessionRecord> cooperative =
        [
            .. all.Where(e => e.SessionType == PlaySessionType.COOPERATIVE && e.Scores.Count > 0),
        ];
        List<SessionRecord> withStartPlayer =
        [
            .. competitive.Where(e =>
                e.StartPlayerId is not null && e.Scores.Any(s => s.PlayerId == e.StartPlayerId)
            ),
        ];

        return new BoardGameStatistics
        {
            PlayCount = all.Count,
            LastPlayedAt = all.Count == 0 ? null : all.Max(e => e.PlayedAt),
            TotalPlayTime = StatisticsMath.TotalPlayTime(all),
            AveragePlayTimes = AveragePlayTimes(all),
            PlayerWinRates = PlayerWinRates(competitiveScores),
            HighScore = HighScore(competitiveScores),
            AverageScore = StatisticsMath.Average([
                .. competitiveScores.Select(e => e.Score.Score).OfType<int>(),
            ]),
            AverageWinningScore = StatisticsMath.Average([
                .. competitiveScores
                    .Where(e => e.Score.Win)
                    .Select(e => e.Score.Score)
                    .OfType<int>(),
            ]),
            StartPlayerWinRate = StatisticsMath.Rate(
                withStartPlayer,
                e => e.Scores.Any(s => s.PlayerId == e.StartPlayerId && s.Win)
            ),
            CooperativeWinRate = StatisticsMath.Rate(cooperative, e => e.Scores.Any(s => s.Win)),
        };
    }

    private static List<PlayTimeByPlayerCount> AveragePlayTimes(List<SessionRecord> sessions)
    {
        return
        [
            .. sessions
                .Where(e => e.Duration is not null)
                .GroupBy(e => e.PlayerIds.Count)
                .OrderBy(group => group.Key)
                .Select(group => new PlayTimeByPlayerCount
                {
                    PlayerCount = group.Key,
                    AveragePlayTime = StatisticsMath.Average([
                        .. group.Select(e => e.Duration!.Value),
                    ]),
                    SessionCount = group.Count(),
                }),
        ];
    }

    private static List<PlayerWinRate> PlayerWinRates(
        List<(SessionRecord Session, ScoreRecord Score)> scores
    )
    {
        return
        [
            .. scores
                .GroupBy(e => e.Score.PlayerId)
                .Select(group =>
                {
                    int playCount = group.Count();
                    int winCount = group.Count(e => e.Score.Win);
                    return new PlayerWinRate
                    {
                        PlayerId = group.Key,
                        PlayerName = group.First().Score.PlayerName,
                        PlayCount = playCount,
                        WinCount = winCount,
                        WinRate = winCount / (double)playCount,
                    };
                })
                .OrderByDescending(e => e.WinRate)
                .ThenByDescending(e => e.WinCount)
                .ThenBy(e => e.PlayerName)
                .ThenBy(e => e.PlayerId),
        ];
    }

    private static HighScore? HighScore(List<(SessionRecord Session, ScoreRecord Score)> scores)
    {
        return scores
            .Where(e => e.Score.Score is not null)
            .OrderByDescending(e => e.Score.Score)
            .ThenBy(e => e.Session.PlayedAt)
            .Select(e => new HighScore
            {
                PlayerId = e.Score.PlayerId,
                PlayerName = e.Score.PlayerName,
                Score = e.Score.Score!.Value,
                SessionId = e.Session.Id,
                PlayedAt = e.Session.PlayedAt,
            })
            .FirstOrDefault();
    }
}
