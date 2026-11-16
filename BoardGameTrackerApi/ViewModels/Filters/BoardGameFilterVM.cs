using BoardGameTracker.Api.ViewModels.Enums;

namespace BoardGameTracker.Api.ViewModels.Filters;

public class BoardGameFilterVM : AbstractFilterVM
{
    public string? Name { get; set; }

    public TimeSpan? MinPlayTime { get; set; }

    public TimeSpan? MaxPlayTime { get; set; }

    public int? MinPlayerCount { get; set; }

    public int? MaxPlayerCount { get; set; }

    public List<BoardGameTypeVM> Type { get; set; } = [];

    public List<BoardGameCategoryVM> Categories { get; set; } = [];
}
