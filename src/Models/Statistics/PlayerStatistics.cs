namespace BoardGameTracker.Models.Statistics;

/// <summary>
/// Calculated play statistics of a player
/// </summary>
public class PlayerStatistics
{
    /// <summary>
    /// Number of sessions of any type the player took part in
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// Time of the player's last session (start time, or creation time without one)
    /// </summary>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Sum of the durations of the player's finished sessions
    /// </summary>
    public TimeSpan TotalPlayTime { get; set; }

    /// <summary>
    /// Number of competitive sessions the player has a score in
    /// </summary>
    public int CompetitivePlayCount { get; set; }

    /// <summary>
    /// Number of competitive sessions the player won
    /// </summary>
    public int WinCount { get; set; }

    /// <summary>
    /// Share of the competitive sessions won, 0 - 1
    /// </summary>
    public double? WinRate { get; set; }

    /// <summary>
    /// Base game the player played the most; the most recently played one on a tie
    /// </summary>
    public MostPlayedGame? MostPlayedGame { get; set; }

    /// <summary>
    /// Highest competitive score of the player per base game, ordered by game name
    /// </summary>
    public List<PersonalBest> PersonalBests { get; set; } = [];
}

/// <summary>
/// Base game with the number of sessions the player took part in
/// </summary>
public class MostPlayedGame
{
    public Guid BoardGameId { get; set; }

    public required string BoardGameName { get; set; }

    public int PlayCount { get; set; }
}

/// <summary>
/// Highest competitive score of a player in a base game; the earliest one on a tie
/// </summary>
public class PersonalBest
{
    public Guid BoardGameId { get; set; }

    public required string BoardGameName { get; set; }

    public int Score { get; set; }

    public Guid SessionId { get; set; }

    public DateTime PlayedAt { get; set; }
}
