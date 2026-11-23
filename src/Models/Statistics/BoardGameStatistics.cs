namespace BoardGameTracker.Models.Statistics;

/// <summary>
/// Calculated play statistics of a board game; an expansion's come from the sessions it was used in
/// </summary>
public class BoardGameStatistics
{
    /// <summary>
    /// Number of sessions of any type
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// Time of the last session (start time, or creation time without one)
    /// </summary>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Sum of the durations of the finished sessions
    /// </summary>
    public TimeSpan TotalPlayTime { get; set; }

    /// <summary>
    /// Average duration of the finished sessions per player count, ordered by player count
    /// </summary>
    public List<PlayTimeByPlayerCount> AveragePlayTimes { get; set; } = [];

    /// <summary>
    /// Win rate of each player in the competitive sessions, best first
    /// </summary>
    public List<PlayerWinRate> PlayerWinRates { get; set; } = [];

    /// <summary>
    /// Highest score of the competitive sessions; the earliest one on a tie
    /// </summary>
    public HighScore? HighScore { get; set; }

    /// <summary>
    /// Average score of the competitive sessions
    /// </summary>
    public double? AverageScore { get; set; }

    /// <summary>
    /// Average score of the winners of the competitive sessions
    /// </summary>
    public double? AverageWinningScore { get; set; }

    /// <summary>
    /// Share of the competitive sessions with a start player that the start player won, 0 - 1
    /// </summary>
    public double? StartPlayerWinRate { get; set; }

    /// <summary>
    /// Share of the cooperative sessions with scores that the team won, 0 - 1
    /// </summary>
    public double? CooperativeWinRate { get; set; }
}

/// <summary>
/// Average duration of the finished sessions with the given player count
/// </summary>
public class PlayTimeByPlayerCount
{
    public int PlayerCount { get; set; }

    public TimeSpan AveragePlayTime { get; set; }

    /// <summary>
    /// Number of finished sessions with the player count
    /// </summary>
    public int SessionCount { get; set; }
}

/// <summary>
/// Wins of a player in the competitive sessions they have a score in
/// </summary>
public class PlayerWinRate
{
    public Guid PlayerId { get; set; }

    public required string PlayerName { get; set; }

    public int PlayCount { get; set; }

    public int WinCount { get; set; }

    /// <summary>
    /// Share of the sessions won, 0 - 1
    /// </summary>
    public double WinRate { get; set; }
}

/// <summary>
/// Highest score of a board game and who scored it
/// </summary>
public class HighScore
{
    public Guid PlayerId { get; set; }

    public required string PlayerName { get; set; }

    public int Score { get; set; }

    public Guid SessionId { get; set; }

    public DateTime PlayedAt { get; set; }
}
