using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models;

/// <summary>
/// Board game information
/// </summary>
public class BoardGame : AbstractEntity
{
    /// <summary>
    /// Name of the board game
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Minimum play time
    /// </summary>
    public required TimeSpan MinPlayTime { get; set; }

    /// <summary>
    /// Maximum playtime
    /// </summary>
    public required TimeSpan MaxPlayTime { get; set; }

    /// <summary>
    /// Minimum number of players
    /// </summary>
    public required int MinPlayerCount { get; set; }

    /// <summary>
    /// Maximum number of players
    /// </summary>
    public required int MaxPlayerCount { get; set; }

    /// <summary>
    /// Optional URL for BoardGameGeek page
    /// </summary>
    public string? BggUrl { get; set; }

    /// <summary>
    /// Type of the board game
    /// </summary>
    public BoardGameType Type { get; set; } = BoardGameType.BASE_GAME;

    /// <summary>
    /// Categories of the board game
    /// </summary>
    public BoardGameCategory Categories { get; set; }

    /// <summary>
    /// Optional id of the board game that this game is the expansion of
    /// </summary>
    public Guid? ExpansionOfId { get; set; }

    /// <summary>
    /// Optional board game entity that this game is the expansion of
    /// </summary>
    public BoardGame? ExpansionOf { get; set; }
}
