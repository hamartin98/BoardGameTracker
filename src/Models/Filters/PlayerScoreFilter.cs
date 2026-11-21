using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Filters;

public class PlayerScoreFilter : AbstractFilter<PlayerScore>
{
    public Guid? PlayerId { get; set; }

    public Guid? SessionId { get; set; }

    public bool? Win { get; set; }

    public override IQueryable<PlayerScore> FilterQuery(IQueryable<PlayerScore> source)
    {
        var filtered = base.FilterQuery(source);
        if (PlayerId is not null)
        {
            filtered = filtered.Where(e => e.PlayerId == PlayerId);
        }
        if (SessionId is not null)
        {
            filtered = filtered.Where(e => e.SessionId == SessionId);
        }
        if (Win is not null)
        {
            filtered = filtered.Where(e => e.Win == Win);
        }
        return filtered;
    }
}
