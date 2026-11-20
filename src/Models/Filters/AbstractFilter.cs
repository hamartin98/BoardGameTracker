using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public abstract class AbstractFilter<EntityType>
    where EntityType : AbstractEntity
{
    public const int MinPage = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 10;

    // Clamped in the setters: a constructor runs before object initializers / Mapster
    // assign the values, so clamping there has no effect.
    private int page = MinPage;
    private int pageSize = DefaultPageSize;

    /// <summary>
    /// 1-based page number; values below 1 are treated as 1
    /// </summary>
    public int Page
    {
        get => page;
        set => page = Math.Max(value, MinPage);
    }

    /// <summary>
    /// Page size, clamped to [1, 100]
    /// </summary>
    public int PageSize
    {
        get => pageSize;
        set => pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
    }

    private DateTime? createdAtFrom;
    private DateTime? createdAtTo;

    /// <summary>
    /// Stored as UTC; a value without a timezone is treated as UTC
    /// </summary>
    public DateTime? CreatedAtFrom
    {
        get => createdAtFrom;
        set => createdAtFrom = ToUtc(value);
    }

    /// <summary>
    /// Stored as UTC; a value without a timezone is treated as UTC
    /// </summary>
    public DateTime? CreatedAtTo
    {
        get => createdAtTo;
        set => createdAtTo = ToUtc(value);
    }

    public string? OrderBy { get; set; }

    public OrderByDirection OrderDirection { get; set; } = OrderByDirection.ASC;

    // Npgsql only accepts UTC DateTimes for timestamp with time zone columns
    private static DateTime? ToUtc(DateTime? value)
    {
        return value?.Kind switch
        {
            null => null,
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc),
        };
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
