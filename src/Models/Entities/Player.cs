using BoardGameTracker.Models.Statistics;

namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Player information
/// </summary>
public class Player : NamedEntity
{
    /// <summary>
    /// Collection of score entities for the player
    /// </summary>
    public ICollection<PlayerScore> PlayerScores { get; } = [];

    /// <summary>
    /// Calculated play statistics; not stored
    /// </summary>
    public PlayerStatistics? Statistics { get; set; }
}
