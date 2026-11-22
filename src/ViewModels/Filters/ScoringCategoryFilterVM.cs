namespace BoardGameTracker.ViewModels.Filters;

public class ScoringCategoryFilterVM : AbstractFilterVM
{
    /// <summary>
    /// Only categories of this board game
    /// </summary>
    public Guid? BoardGameId { get; set; }

    /// <summary>
    /// Only categories whose name contains this text (case-insensitive)
    /// </summary>
    public string? Name { get; set; }
}
