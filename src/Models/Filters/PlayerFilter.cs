using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Filters;

public class PlayerFilter : AbstractFilter<Player>
{
    public string? Name { get; set; }

    public override IQueryable<Player> FilterQuery(IQueryable<Player> source)
    {
        var filtered = base.FilterQuery(source);
        if (Name is not null)
        {
            filtered = filtered.Where(e => e.NormalizedName.Contains(Name.ToLower()));
        }
        return filtered;
    }
}
