using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public class BoardGameFilter : AbstractFilter<BoardGame>
{
    public string? Name { get; set; }

    public TimeSpan? MinPlayTime { get; set; }

    public TimeSpan? MaxPlayTime { get; set; }

    public int? MinPlayerCount { get; set; }

    public int? MaxPlayerCount { get; set; }

    public List<BoardGameType> Types { get; set; } = [];

    public List<BoardGameCategory> Categories { get; set; } = [];

    public override IQueryable<BoardGame> FilterQuery(IQueryable<BoardGame> source)
    {
        var filtered = base.FilterQuery(source);
        if (Name is not null)
        {
            filtered = filtered.Where(e => e.NormalizedName.Contains(Name.ToLower()));
        }
        if (MinPlayTime is not null)
        {
            filtered = filtered.Where(e => e.MinPlayTime >= MinPlayTime);
        }
        if (MaxPlayTime is not null)
        {
            filtered = filtered.Where(e => e.MaxPlayTime <= MaxPlayTime);
        }
        if (MinPlayerCount is not null)
        {
            filtered = filtered.Where(e => e.MinPlayerCount >= MinPlayerCount);
        }
        if (MaxPlayerCount is not null)
        {
            filtered = filtered.Where(e => e.MaxPlayerCount <= MaxPlayerCount);
        }
        if (Types.Any())
        {
            filtered = filtered.Where(e => Types.Contains(e.Type));
        }
        if (Categories is not null)
        {
            long combinedFlags = Categories.Aggregate(0L, (acc, category) => acc | (long)category);
            filtered = filtered.Where(e => ((long)e.Categories & combinedFlags) != 0);
        }
        return filtered;
    }
}
