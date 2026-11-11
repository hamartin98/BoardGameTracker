using BoardGameTracker.Models.Entities;

/// <summary>
/// Recorded score for a player for the given session
/// </summary>
public class PlayerScore : AbstractEntity
{
    /// <summary>
    /// Id of the player the score entity belongs to
    /// </summary>
    public Guid PlayerId { get; set; }

    /// <summary>
    /// Player entity the score belongs to
    /// </summary>
    public Player? Player { get; set; }

    /// <summary>
    /// Id of the session the scoring belongs to
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Session entity the scoring belongs to
    /// </summary>
    public PlaySession? Session { get; set; }

    /// <summary>
    /// Score of the player in the given session (null when no scoring is applicable)
    /// </summary>
    public int? Score { get; set; }

    /// <summary>
    /// Whether the player won the game
    /// </summary>
    public bool Win { get; set; }

    /// <summary>
    /// Additional information about the score entry (e.g. player color, asymmetric player info)
    /// </summary>
    public string? Notes { get; set; }
}
