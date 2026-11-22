using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Filters;

public class ScoringCategoryFilter : AbstractFilter<ScoringCategory>
{
    public Guid? BoardGameId { get; set; }

    public string? Name { get; set; }

    public override IQueryable<ScoringCategory> FilterQuery(IQueryable<ScoringCategory> source)
    {
        var filtered = base.FilterQuery(source);
        if (BoardGameId is not null)
        {
            filtered = filtered.Where(e => e.BoardGameId == BoardGameId);
        }
        if (Name is not null)
        {
            string normalizedName = Name.ToLowerInvariant();
            filtered = filtered.Where(e => e.NormalizedName.Contains(normalizedName));
        }
        return filtered;
    }
}
