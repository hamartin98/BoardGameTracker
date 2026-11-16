using System.ComponentModel;

namespace BoardGameTracker.Api.ViewModels.Filters;

public abstract class AbstractFilterVM
{
    public int Page { get; set; }

    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
}
