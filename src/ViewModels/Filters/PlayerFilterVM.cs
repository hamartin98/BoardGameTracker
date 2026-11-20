namespace BoardGameTracker.ViewModels.Filters;

public class PlayerFilterVM : AbstractFilterVM
{
    /// <summary>
    /// Case-insensitive substring of the player name
    /// </summary>
    public string? Name { get; set; }
}
