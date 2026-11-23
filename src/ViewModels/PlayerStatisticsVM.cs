namespace BoardGameTracker.ViewModels;

/// <summary>
/// Calculated play statistics of a player
/// </summary>
public class PlayerStatisticsVM
{
    /// <summary>
    /// Number of sessions of any type the player took part in
    /// </summary>
    /// <example>20</example>
    public int PlayCount { get; set; }

    /// <summary>
    /// Time of the player's last session (start time, or creation time without one)
    /// </summary>
    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Sum of the durations of the player's finished sessions
    /// </summary>
    /// <example>1.12:00:00</example>
    public TimeSpan TotalPlayTime { get; set; }

    /// <summary>
    /// Number of competitive sessions the player has a score in
    /// </summary>
    /// <example>15</example>
    public int CompetitivePlayCount { get; set; }

    /// <summary>
    /// Number of competitive sessions the player won
    /// </summary>
    /// <example>6</example>
    public int WinCount { get; set; }

    /// <summary>
    /// Share of the competitive sessions won, 0 - 1
    /// </summary>
    /// <example>0.4</example>
    public double? WinRate { get; set; }

    /// <summary>
    /// Base game the player played the most; the most recently played one on a tie
    /// </summary>
    public MostPlayedGameVM? MostPlayedGame { get; set; }

    /// <summary>
    /// Highest competitive score of the player per base game, ordered by game name
    /// </summary>
    public List<PersonalBestVM> PersonalBests { get; set; } = [];
}

/// <summary>
/// Base game with the number of sessions the player took part in
/// </summary>
public class MostPlayedGameVM
{
    public Guid BoardGameId { get; set; }

    /// <example>Scythe</example>
    public required string BoardGameName { get; set; }

    /// <example>8</example>
    public int PlayCount { get; set; }
}

/// <summary>
/// Highest competitive score of a player in a base game; the earliest one on a tie
/// </summary>
public class PersonalBestVM
{
    public Guid BoardGameId { get; set; }

    /// <example>Scythe</example>
    public required string BoardGameName { get; set; }

    /// <example>92</example>
    public int Score { get; set; }

    public Guid SessionId { get; set; }

    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime PlayedAt { get; set; }
}
