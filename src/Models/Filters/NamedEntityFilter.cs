using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Filters;

public abstract class NamedEntityFilter<EntityType> : AbstractFilter<EntityType>
    where EntityType : NamedEntity
{
    public string? Name { get; set; }

    public override IQueryable<EntityType> FilterQuery(IQueryable<EntityType> source)
    {
        var filtered = base.FilterQuery(source);
        if (Name is not null)
        {
            string normalizedName = Name.ToLowerInvariant();
            filtered = filtered.Where(e => e.NormalizedName.Contains(normalizedName));
        }
        return filtered;
    }
}
