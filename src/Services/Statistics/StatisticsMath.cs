namespace BoardGameTracker.Services.Statistics;

internal static class StatisticsMath
{
    /// <summary>
    /// Share of the matching items, 0 - 1; null without items
    /// </summary>
    public static double? Rate<T>(IReadOnlyCollection<T> items, Func<T, bool> predicate)
    {
        return items.Count == 0 ? null : items.Count(predicate) / (double)items.Count;
    }

    public static double? Average(IReadOnlyCollection<int> values)
    {
        return values.Count == 0 ? null : values.Average();
    }

    public static TimeSpan Average(IReadOnlyCollection<TimeSpan> values)
    {
        return TimeSpan.FromTicks((long)values.Average(e => e.Ticks));
    }

    /// <summary>
    /// Sum of the durations of the finished sessions
    /// </summary>
    public static TimeSpan TotalPlayTime(IEnumerable<SessionRecord> sessions)
    {
        return TimeSpan.FromTicks(sessions.Sum(e => e.Duration?.Ticks ?? 0));
    }
}
