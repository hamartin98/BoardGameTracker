namespace BoardGameTracker.ViewModels;

/// <summary>
/// Recommended board game with its score and the parts the score is made of
/// </summary>
public class BoardGameRecommendationVM
{
    /// <summary>
    /// Recommended board game
    /// </summary>
    public required BoardGameResponseVM BoardGame { get; set; }

    /// <summary>
    /// Total score, 0 - 100: the weighted average of the used score parts
    /// </summary>
    /// <example>82.5</example>
    public double Score { get; set; }

    /// <summary>
    /// Play time fit, 0 - 1, from the game's min/max play time and the average session play time; null without a requested play time
    /// </summary>
    /// <example>0.9</example>
    public double? TimeScore { get; set; }

    /// <summary>
    /// Score for the time since the last play, 0 - 1; 1 for a game never played
    /// </summary>
    /// <example>0.75</example>
    public double RecencyScore { get; set; }

    /// <summary>
    /// Share of the requested categories the game has, 0 - 1; null without requested categories
    /// </summary>
    /// <example>0.5</example>
    public double? CategoryScore { get; set; }

    /// <summary>
    /// Average duration of the finished sessions with the requested player count
    /// </summary>
    /// <example>01:45:00</example>
    public TimeSpan? AveragePlayTime { get; set; }

    /// <summary>
    /// Time of the last session of the game
    /// </summary>
    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Number of sessions of the game with any player count
    /// </summary>
    /// <example>3</example>
    public int SessionCount { get; set; }
}
