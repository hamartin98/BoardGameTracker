using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public abstract class AbstractFilter<EntityType>
    where EntityType : AbstractEntity
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public DateTime? CreatedAtFrom { get; set; }

    public DateTime? CreatedAtTo { get; set; }

    public string? OrderBy { get; set; }

    public OrderByDirection OrderDirection { get; set; } = OrderByDirection.ASC;

    public AbstractFilter()
    {
        Page = Page < 1 ? 1 : Page;
        PageSize = PageSize < 1 ? 1 : PageSize;
        PageSize = PageSize > 100 ? 100 : PageSize;
    }

    public virtual IQueryable<EntityType> FilterQuery(IQueryable<EntityType> source)
    {
        var filtered = source;

        if (OrderBy is not null)
        {
            filtered = filtered.OrderBy(OrderBy, OrderDirection).ThenBy("id", OrderByDirection.ASC);
        }
        else
        {
            filtered = filtered.OrderBy("id", OrderByDirection.ASC);
        }

        if (CreatedAtFrom.HasValue)
        {
            filtered = filtered.Where(e => e.CreatedAt >= CreatedAtFrom);
        }
        if (CreatedAtTo.HasValue)
        {
            filtered = filtered.Where(e => e.CreatedAt <= CreatedAtTo);
        }
        return filtered;
    }
}
