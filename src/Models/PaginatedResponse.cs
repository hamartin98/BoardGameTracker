using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Models;

public class PaginatedResponse<EntityType>
{
    public List<EntityType> Items { get; }

    /// <summary>
    /// 1-based page number
    /// </summary>
    public int Page { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public PaginatedResponse(
        IEnumerable<EntityType> currentPageItems,
        int count,
        int page,
        int pageSize
    )
    {
        Page = page;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        Items = [.. currentPageItems];
    }

    public static async Task<PaginatedResponse<EntityType>> CreateAsync(
        IQueryable<EntityType> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PaginatedResponse<EntityType>(items, count, page, pageSize);
    }
}
