using System.ComponentModel;
using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.ViewModels.Filters;

public abstract class AbstractFilterVM
{
    /// <summary>
    /// 1-based page number
    /// </summary>
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }

    public string? OrderBy { get; set; }

    public OrderByDirectionVM OrderDirection { get; set; } = OrderByDirectionVM.ASC;
}
