using System.ComponentModel.DataAnnotations;

namespace BoardGameTracker.ViewModels;

public class ScoringCategoryBaseVM
{
    /// <summary>
    /// Name of the category, unique within the board game
    /// </summary>
    /// <example>Stars</example>
    [Required]
    public required string Name { get; set; }
}

/// <summary>
/// Scoring category create request; the category is added to the end of the board game's scoring order
/// </summary>
public class ScoringCategoryCreateVM : ScoringCategoryBaseVM
{
    /// <summary>
    /// Id of the board game (base game or expansion) the category belongs to
    /// </summary>
    public required Guid BoardGameId { get; set; }
}

/// <summary>
/// Scoring category update request; the board game and the order cannot be changed here
/// </summary>
public class ScoringCategoryUpdateVM : ScoringCategoryBaseVM { }

public class ScoringCategoryResponseVM : ScoringCategoryCreateVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }

    /// <summary>
    /// 0-based position of the category in the board game's scoring order
    /// </summary>
    /// <example>0</example>
    public int Order { get; set; }
}

public class ScoringCategoriesResponseVM : PaginatedResponseVM<ScoringCategoryResponseVM>;
