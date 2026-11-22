using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Recommendations;

/// <summary>
/// Recommended board game with its score and the parts the score is made of
/// </summary>
public class BoardGameRecommendation
{
    /// <summary>
    /// Recommended board game
    /// </summary>
    public required BoardGame BoardGame { get; set; }

    /// <summary>
    /// Total score, 0 - 100
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Play time fit, 0 - 1; null when no play time range was requested
    /// </summary>
    public double? TimeScore { get; set; }

    /// <summary>
    /// Score for the time since the last play, 0 - 1
    /// </summary>
    public double RecencyScore { get; set; }

    /// <summary>
    /// Share of the requested categories the game has, 0 - 1; null when no categories were requested
    /// </summary>
    public double? CategoryScore { get; set; }

    /// <summary>
    /// Average duration of the finished sessions with the requested player count
    /// </summary>
    public TimeSpan? AveragePlayTime { get; set; }

    /// <summary>
    /// Time of the last session of the game
    /// </summary>
    public DateTime? LastPlayedAt { get; set; }

    /// <summary>
    /// Number of sessions of the game with any player count
    /// </summary>
    public int SessionCount { get; set; }
}
