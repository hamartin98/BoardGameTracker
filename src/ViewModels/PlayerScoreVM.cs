using System.ComponentModel.DataAnnotations;

namespace BoardGameTracker.ViewModels;

/// <summary>
/// Points of a player in one scoring category
/// </summary>
public class CategoryScoreVM
{
    /// <summary>
    /// Id of the scoring category
    /// </summary>
    public required Guid ScoringCategoryId { get; set; }

    /// <summary>
    /// Points in the category
    /// </summary>
    /// <example>24</example>
    public int Value { get; set; }
}

public class PlayerScoreBaseVM : IValidatableObject
{
    /// <summary>
    /// Whether the player won the game
    /// </summary>
    /// <example>true</example>
    public bool Win { get; set; }

    /// <summary>
    /// Additional information about the score entry (e.g. player color, asymmetric player info)
    /// </summary>
    /// <example>Polania</example>
    public string? Notes { get; set; }

    /// <summary>
    /// Points for every scoring category of the session's board game and used expansions; empty when it has none
    /// </summary>
    public ICollection<CategoryScoreVM> CategoryScores { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CategoryScores.DistinctBy(e => e.ScoringCategoryId).Count() != CategoryScores.Count)
        {
            yield return new("Each scoring category can only be scored once.");
        }
    }
}

public class PlayerScoreCreateVM : PlayerScoreBaseVM
{
    /// <summary>
    /// Id of the player the score belongs to
    /// </summary>
    public required Guid PlayerId { get; set; }

    /// <summary>
    /// Id of the play session the score belongs to
    /// </summary>
    public required Guid SessionId { get; set; }
}

/// <summary>
/// Player score update request; the player and session cannot be changed
/// </summary>
public class PlayerScoreUpdateVM : PlayerScoreBaseVM { }

public class PlayerScoreResponseVM : PlayerScoreCreateVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }

    /// <summary>
    /// Total score: the sum of the category scores, or a total recorded without a breakdown; null without points
    /// </summary>
    /// <example>78</example>
    public int? Score { get; set; }
}

public class PlayerScoresResponseVM : PaginatedResponseVM<PlayerScoreResponseVM>;
