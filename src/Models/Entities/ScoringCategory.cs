namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Scoring category of a board game, in the order it is scored
/// </summary>
public class ScoringCategory : AbstractEntity
{
    /// <summary>
    /// Id of the board game (base game or expansion) the category belongs to
    /// </summary>
    public required Guid BoardGameId { get; set; }

    /// <summary>
    /// Board game entity the category belongs to
    /// </summary>
    public BoardGame? BoardGame { get; set; }

    /// <summary>
    /// Name of the category, unique (case-insensitive) within the board game
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Lowercase name, used for uniqueness and search
    /// </summary>
    public string NormalizedName
    {
        get => Name.ToLowerInvariant();
        set { }
    }

    /// <summary>
    /// 0-based position of the category in the board game's scoring order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Scores recorded in this category
    /// </summary>
    public ICollection<CategoryScore> CategoryScores { get; } = [];
}
