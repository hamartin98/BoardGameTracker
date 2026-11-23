using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Board game information
/// </summary>
public class BoardGame : NamedEntity
{
    /// <summary>
    /// Optional minimum play time
    /// </summary>
    public TimeSpan? MinPlayTime { get; set; }

    /// <summary>
    /// Optional maximum play time
    /// </summary>
    public TimeSpan? MaxPlayTime { get; set; }

    /// <summary>
    /// Minimum number of players
    /// </summary>
    public required byte MinPlayerCount { get; set; }

    /// <summary>
    /// Maximum number of players
    /// </summary>
    public required byte MaxPlayerCount { get; set; }

    /// <summary>
    /// Optional BoardGameGeek id
    /// </summary>
    public int? BggId { get; set; }

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

    /// <summary>
    /// Expansions of the game
    /// </summary>
    public ICollection<BoardGame> Expansions { get; } = [];

    /// <summary>
    /// Scoring categories of the game; an expansion's categories extend its base game's
    /// </summary>
    public ICollection<ScoringCategory> ScoringCategories { get; } = [];
}
