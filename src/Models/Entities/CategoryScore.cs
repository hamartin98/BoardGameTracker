namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Points of a player score in one scoring category; part of the player score and versioned with it
/// </summary>
public class CategoryScore
{
    /// <summary>
    /// Id of the player score the points belong to
    /// </summary>
    public Guid PlayerScoreId { get; set; }

    /// <summary>
    /// Player score entity the points belong to
    /// </summary>
    public PlayerScore? PlayerScore { get; set; }

    /// <summary>
    /// Id of the scoring category
    /// </summary>
    public Guid ScoringCategoryId { get; set; }

    /// <summary>
    /// Scoring category entity
    /// </summary>
    public ScoringCategory? ScoringCategory { get; set; }

    /// <summary>
    /// Points in the category
    /// </summary>
    public int Value { get; set; }
}
