namespace BoardGameTracker.Api.ViewModels;

public class PaginatedResponseVM<ResponseType>
{
    /// <summary>
    /// Current page number
    /// </summary>
    /// <example>0</example>
    public int Page { get; set; }

    /// <summary>
    /// Number of elements on page
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of entities with the given filtering
    /// </summary>
    /// <example>99</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// List of result items
    /// </summary>
    /// <example>[]</example>
    public List<ResponseType> Items { get; set; } = [];
}
