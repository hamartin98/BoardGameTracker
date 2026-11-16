namespace BoardGameTracker.Models;

using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

public class PaginatedResponse<EntityType> : List<EntityType>
{
    public int CurrentPage { get; set; }

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
        CurrentPage = page;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        AddRange(currentPageItems);
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
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PaginatedResponse<EntityType>(items, count, page, pageSize);
    }
}
