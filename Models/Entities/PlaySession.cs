using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Recorded play session for a board game with scores
/// </summary>
public class PlaySession : AbstractEntity
{
  /// <summary>
  /// Id of the board game used
  /// </summary>
    public required Guid BoardGameId { get; set; }

    /// <summary>
    /// Used board game entity
    /// </summary>
    public BoardGame? BoardGame { get; set; }

    /// <summary>
    /// List of expansion ids used during the session
    /// </summary>
    public ICollection<Guid> UsedExpansionIds { get; set; } = [];

    /// <summary>
    /// List of used expansion entities used during the session
    /// </summary>
    public ICollection<BoardGame> UsedExpansions { get; set; } = [];

    /// <summary>
    /// Starting date and time of the play session
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Total duration of the play session
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Player score ids for the play session
    /// </summary>
    public ICollection<Guid> PlayerScoreIds { get; set; } = [];

    /// <summary>
    /// Player scores for the play session
    /// </summary>
    public ICollection<PlayerScore> PlayerScores { get; set; } = [];

    /// <summary>
    /// Type of the play session
    /// </summary>
    public PlaySessionType SessionType {get; set; }

    /// <summary>
    /// Additional notes about the play session
    /// </summary>
    public string? Notes { get; set; }
}
