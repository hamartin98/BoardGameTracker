using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;

namespace BoardGameTracker.Models.Filters;

public class PlaySessionFilter : AbstractFilter<PlaySession>
{
    private DateTime? startTimeFrom;
    private DateTime? startTimeTo;

    public Guid? BoardGameId { get; set; }

    public List<PlaySessionType> SessionTypes { get; set; } = [];

    public DateTime? StartTimeFrom
    {
        get => startTimeFrom;
        set => startTimeFrom = value.ToUtc();
    }

    public DateTime? StartTimeTo
    {
        get => startTimeTo;
        set => startTimeTo = value.ToUtc();
    }

    public override IQueryable<PlaySession> FilterQuery(IQueryable<PlaySession> source)
    {
        var filtered = base.FilterQuery(source);
        if (BoardGameId is not null)
        {
            filtered = filtered.Where(e => e.BoardGameId == BoardGameId);
        }
        if (SessionTypes.Count != 0)
        {
            filtered = filtered.Where(e => SessionTypes.Contains(e.SessionType));
        }
        if (StartTimeFrom is not null)
        {
            filtered = filtered.Where(e => e.StartTime >= StartTimeFrom);
        }
        if (StartTimeTo is not null)
        {
            filtered = filtered.Where(e => e.StartTime <= StartTimeTo);
        }
        return filtered;
    }
}
