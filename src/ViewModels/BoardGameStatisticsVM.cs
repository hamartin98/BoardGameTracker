namespace BoardGameTracker.ViewModels;

/// <summary>
/// Calculated play statistics of a board game; an expansion's come from the sessions it was used in
/// </summary>
public class BoardGameStatisticsVM
{
    /// <summary>
    /// Number of sessions of any type
    /// </summary>
    /// <example>12</example>
    public int PlayCount { get; set; }

    /// <summary>
    /// Time of the last session (start time, or creation time without one)
    /// </summary>
    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Sum of the durations of the finished sessions
    /// </summary>
    /// <example>1.02:30:00</example>
    public TimeSpan TotalPlayTime { get; set; }

    /// <summary>
    /// Average duration of the finished sessions per player count, ordered by player count
    /// </summary>
    public List<PlayTimeByPlayerCountVM> AveragePlayTimes { get; set; } = [];

    /// <summary>
    /// Win rate of each player in the competitive sessions, best first
    /// </summary>
    public List<PlayerWinRateVM> PlayerWinRates { get; set; } = [];

    /// <summary>
    /// Highest score of the competitive sessions; the earliest one on a tie
    /// </summary>
    public HighScoreVM? HighScore { get; set; }

    /// <summary>
    /// Average score of the competitive sessions
    /// </summary>
    /// <example>71.5</example>
    public double? AverageScore { get; set; }

    /// <summary>
    /// Average score of the winners of the competitive sessions
    /// </summary>
    /// <example>80.25</example>
    public double? AverageWinningScore { get; set; }

    /// <summary>
    /// Share of the competitive sessions with a start player that the start player won, 0 - 1
    /// </summary>
    /// <example>0.4</example>
    public double? StartPlayerWinRate { get; set; }

    /// <summary>
    /// Share of the cooperative sessions with scores that the team won, 0 - 1
    /// </summary>
    /// <example>0.6</example>
    public double? CooperativeWinRate { get; set; }
}

/// <summary>
/// Average duration of the finished sessions with the given player count
/// </summary>
public class PlayTimeByPlayerCountVM
{
    /// <example>3</example>
    public int PlayerCount { get; set; }

    /// <example>01:45:00</example>
    public TimeSpan AveragePlayTime { get; set; }

    /// <summary>
    /// Number of finished sessions with the player count
    /// </summary>
    /// <example>4</example>
    public int SessionCount { get; set; }
}

/// <summary>
/// Wins of a player in the competitive sessions they have a score in
/// </summary>
public class PlayerWinRateVM
{
    public Guid PlayerId { get; set; }

    /// <example>Alice</example>
    public required string PlayerName { get; set; }

    /// <example>5</example>
    public int PlayCount { get; set; }

    /// <example>2</example>
    public int WinCount { get; set; }

    /// <summary>
    /// Share of the sessions won, 0 - 1
    /// </summary>
    /// <example>0.4</example>
    public double WinRate { get; set; }
}

/// <summary>
/// Highest score of a board game and who scored it
/// </summary>
public class HighScoreVM
{
    public Guid PlayerId { get; set; }

    /// <example>Alice</example>
    public required string PlayerName { get; set; }

    /// <example>92</example>
    public int Score { get; set; }

    public Guid SessionId { get; set; }

    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime PlayedAt { get; set; }
}
