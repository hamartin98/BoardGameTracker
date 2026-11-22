using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Recommendations;

/// <summary>
/// Board game recommendation request
/// </summary>
public class RecommendationRequest
{
    /// <summary>
    /// Number of players
    /// </summary>
    public byte PlayerCount { get; set; }

    /// <summary>
    /// Lower bound of the wanted play time; open when null
    /// </summary>
    public TimeSpan? MinPlayTime { get; set; }

    /// <summary>
    /// Upper bound of the wanted play time; open when null
    /// </summary>
    public TimeSpan? MaxPlayTime { get; set; }

    /// <summary>
    /// Wanted categories; matching games score higher
    /// </summary>
    public BoardGameCategory Categories { get; set; }

    /// <summary>
    /// Maximum number of recommendations; the configured default when null
    /// </summary>
    public int? Limit { get; set; }
}
