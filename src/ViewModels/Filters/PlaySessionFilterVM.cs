using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.ViewModels.Filters;

public class PlaySessionFilterVM : AbstractFilterVM
{
    /// <summary>
    /// Only sessions of this board game
    /// </summary>
    public Guid? BoardGameId { get; set; }

    /// <summary>
    /// Only sessions of these types
    /// </summary>
    public List<PlaySessionTypeVM> SessionTypes { get; set; } = [];

    /// <summary>
    /// Sessions starting at or after this time
    /// </summary>
    public DateTime? StartTimeFrom { get; set; }

    /// <summary>
    /// Sessions starting at or before this time
    /// </summary>
    public DateTime? StartTimeTo { get; set; }
}
