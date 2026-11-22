namespace BoardGameTracker.Models.Recommendations;

/// <summary>
/// Weights and limits of the board game recommendation score, bound from the RecommendationConfig section
/// </summary>
public class RecommendationOptions
{
    /// <summary>
    /// Weight of the play time fit in the total score
    /// </summary>
    public double TimeWeight { get; set; } = 0.4;

    /// <summary>
    /// Weight of the time since the game was last played in the total score
    /// </summary>
    public double RecencyWeight { get; set; } = 0.35;

    /// <summary>
    /// Weight of the requested category matches in the total score
    /// </summary>
    public double CategoryWeight { get; set; } = 0.25;

    /// <summary>
    /// Share of the average session play time in the time fit; the rest is the game's min/max play time
    /// </summary>
    public double AverageWeight { get; set; } = 0.6;

    /// <summary>
    /// Days since the last play after which the recency score reaches 0.5
    /// </summary>
    public double HalfLifeDays { get; set; } = 60;

    /// <summary>
    /// Distance outside the requested time range at which the time fit drops to about 0.37 (1/e)
    /// </summary>
    public TimeSpan TimeTolerance { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Number of recommendations returned when no limit is requested
    /// </summary>
    public int DefaultLimit { get; set; } = 10;

    /// <summary>
    /// Upper bound of the requested limit
    /// </summary>
    public int MaxLimit { get; set; } = 50;
}
