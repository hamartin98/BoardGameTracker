using System.ComponentModel.DataAnnotations;
using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.ViewModels.Filters;

/// <summary>
/// Board game recommendation request; only the player count is required
/// </summary>
public class RecommendationFilterVM : IValidatableObject
{
    /// <summary>
    /// Number of players; only games supporting it are recommended
    /// </summary>
    /// <example>2</example>
    [Required]
    [Range(1, 100)]
    public byte? PlayerCount { get; set; }

    /// <summary>
    /// Lower bound of the wanted play time; games fitting the range score higher
    /// </summary>
    /// <example>01:00:00</example>
    public TimeSpan? MinPlayTime { get; set; }

    /// <summary>
    /// Upper bound of the wanted play time; games fitting the range score higher
    /// </summary>
    /// <example>02:00:00</example>
    public TimeSpan? MaxPlayTime { get; set; }

    /// <summary>
    /// Wanted categories; games having more of them score higher
    /// </summary>
    public List<BoardGameCategoryVM> Categories { get; set; } = [];

    /// <summary>
    /// Maximum number of recommendations; capped by the server's configured maximum
    /// </summary>
    /// <example>5</example>
    [Range(1, int.MaxValue)]
    public int? Limit { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPlayTime is not null && MinPlayTime <= TimeSpan.Zero)
        {
            yield return new("MinPlayTime should be positive.");
        }
        if (MaxPlayTime is not null && MaxPlayTime <= TimeSpan.Zero)
        {
            yield return new("MaxPlayTime should be positive.");
        }
        if (MinPlayTime > MaxPlayTime)
        {
            yield return new("MinPlayTime should be less than MaxPlayTime.");
        }
    }
}
