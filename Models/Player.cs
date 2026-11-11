namespace BoardGameTracker.Models;

/// <summary>
/// Player information
/// </summary>
public class Player : AbstractEntity
{
    /// <summary>
    /// Name of the player
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Collection of score entity ids for the player
    /// </summary>
    public ICollection<Guid> PlayerScoreIds { get; set; } = [];

    /// <summary>
    /// Collection of score entities for the player
    /// </summary>
    public ICollection<PlayerScore> PlayerScores { get; set; } = [];
}
