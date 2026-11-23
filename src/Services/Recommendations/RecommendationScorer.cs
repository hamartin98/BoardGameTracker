using System.Numerics;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Recommendations;

namespace BoardGameTracker.Services.Recommendations;

/// <summary>
/// Scores board games for a recommendation request; every part is in the range 0 - 1
/// </summary>
public static class RecommendationScorer
{
    public static BoardGameRecommendation Score(
        BoardGame boardGame,
        SessionStatistics statistics,
        RecommendationRequest request,
        RecommendationOptions options,
        DateTime now
    )
    {
        double? timeScore = null;
        if (request.MinPlayTime is not null || request.MaxPlayTime is not null)
        {
            TimeSpan? gameMin = boardGame.MinPlayTime ?? boardGame.MaxPlayTime;
            TimeSpan? gameMax = boardGame.MaxPlayTime ?? boardGame.MinPlayTime;
            double? rangeFit =
                gameMin is TimeSpan from && gameMax is TimeSpan to
                    ? RangeFit(
                        from,
                        to,
                        request.MinPlayTime,
                        request.MaxPlayTime,
                        options.TimeTolerance
                    )
                    : null;
            double? averageFit = statistics.AveragePlayTime is TimeSpan average
                ? PointFit(average, request.MinPlayTime, request.MaxPlayTime, options.TimeTolerance)
                : null;
            timeScore = rangeFit is double range
                ? TimeScore(range, averageFit, options.AverageWeight)
                : averageFit;
        }

        double recencyScore = RecencyScore(statistics.LastPlayedAt, now, options.HalfLifeDays);
        double? categoryScore =
            request.Categories == BoardGameCategory.NONE
                ? null
                : CategoryScore(boardGame.Categories, request.Categories);

        return new()
        {
            BoardGame = boardGame,
            Score = Total(
                (options.TimeWeight, timeScore),
                (options.RecencyWeight, recencyScore),
                (options.CategoryWeight, categoryScore)
            ),
            TimeScore = timeScore,
            RecencyScore = recencyScore,
            CategoryScore = categoryScore,
            AveragePlayTime = statistics.AveragePlayTime,
            LastPlayedAt = statistics.LastPlayedAt,
            SessionCount = statistics.SessionCount,
        };
    }

    /// <summary>
    /// 1 inside the requested range, fading exponentially with the distance outside it
    /// </summary>
    public static double PointFit(TimeSpan time, TimeSpan? min, TimeSpan? max, TimeSpan tolerance)
    {
        if (min is not null && time < min)
        {
            return Math.Exp(-(min.Value - time) / tolerance);
        }
        if (max is not null && time > max)
        {
            return Math.Exp(-(time - max.Value) / tolerance);
        }
        return 1;
    }

    /// <summary>
    /// Average of PointFit over the game's play time range
    /// </summary>
    public static double RangeFit(
        TimeSpan gameMin,
        TimeSpan gameMax,
        TimeSpan? min,
        TimeSpan? max,
        TimeSpan tolerance
    )
    {
        if (gameMax <= gameMin)
        {
            return PointFit(gameMin, min, max, tolerance);
        }

        double tau = tolerance.TotalMinutes;
        double from = gameMin.TotalMinutes;
        double to = gameMax.TotalMinutes;
        double low = min?.TotalMinutes ?? double.NegativeInfinity;
        double high = max?.TotalMinutes ?? double.PositiveInfinity;

        // Inside the range PointFit is 1; outside, the exponential is integrated over the distance
        double inside = Math.Max(0, Math.Min(to, high) - Math.Max(from, low));
        double below = from < low ? FadeIntegral(low - Math.Min(to, low), low - from, tau) : 0;
        double above = to > high ? FadeIntegral(Math.Max(from, high) - high, to - high, tau) : 0;
        return (inside + below + above) / (to - from);
    }

    public static double TimeScore(double rangeFit, double? averageFit, double averageWeight)
    {
        return averageFit is double average
            ? averageWeight * average + (1 - averageWeight) * rangeFit
            : rangeFit;
    }

    /// <summary>
    /// 1 for a game never played, otherwise 1 - 0.5^(days since the last play / half-life)
    /// </summary>
    public static double RecencyScore(DateTime? lastPlayedAt, DateTime now, double halfLifeDays)
    {
        if (lastPlayedAt is null)
        {
            return 1;
        }
        double days = Math.Max(0, (now - lastPlayedAt.Value).TotalDays);
        return 1 - Math.Pow(0.5, days / halfLifeDays);
    }

    /// <summary>
    /// Share of the requested category flags the game has
    /// </summary>
    public static double CategoryScore(BoardGameCategory game, BoardGameCategory requested)
    {
        int requestedCount = BitOperations.PopCount((ulong)requested);
        return (double)BitOperations.PopCount((ulong)(game & requested)) / requestedCount;
    }

    /// <summary>
    /// Weighted average of the used parts (null parts are left out), scaled to 0 - 100 and rounded to 1 decimal
    /// </summary>
    public static double Total(params (double Weight, double? Score)[] parts)
    {
        var used = parts.Where(e => e.Score is not null).ToList();
        double weights = used.Sum(e => e.Weight);
        if (weights <= 0)
        {
            return 0;
        }
        double total = used.Sum(e => e.Weight * e.Score!.Value) / weights;
        return Math.Round(total * 100, 1);
    }

    /// <summary>
    /// Integral of exp(-d / tau) for d from fromDistance to toDistance
    /// </summary>
    private static double FadeIntegral(double fromDistance, double toDistance, double tau)
    {
        return tau * (Math.Exp(-fromDistance / tau) - Math.Exp(-toDistance / tau));
    }
}
