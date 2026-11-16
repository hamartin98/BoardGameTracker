namespace BoardGameTracker.Api.ViewModels;

public class PaginatedResponseVM<ResponseType>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<ResponseType> Items { get; set; } = [];
}
