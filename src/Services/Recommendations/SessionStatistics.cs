namespace BoardGameTracker.Services.Recommendations;

/// <summary>
/// Play session statistics of a board game used by the recommendation score
/// </summary>
/// <param name="LastPlayedAt">Time of the last session</param>
/// <param name="AveragePlayTime">Average duration of the finished sessions with the requested player count</param>
/// <param name="SessionCount">Number of sessions with any player count</param>
public record SessionStatistics(DateTime? LastPlayedAt, TimeSpan? AveragePlayTime, int SessionCount)
{
    /// <summary>
    /// Statistics of a game that was never played
    /// </summary>
    public static readonly SessionStatistics NeverPlayed = new(null, null, 0);
}
