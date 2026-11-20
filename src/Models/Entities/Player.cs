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
}
