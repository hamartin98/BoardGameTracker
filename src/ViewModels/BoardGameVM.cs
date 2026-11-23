using System.ComponentModel.DataAnnotations;
using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.ViewModels;

public class BoardGameBaseVM : IValidatableObject
{
    /// <summary>
    /// Name of the board game
    /// </summary>
    /// <example>Scythe</example>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// Minimum play time
    /// </summary>
    /// <example>00:10:00</example>
    [Required]
    public required TimeSpan MinPlayTime { get; set; }

    /// <summary>
    /// Maximum playtime
    /// </summary>
    /// <example>1:00:00</example>
    [Required]
    public required TimeSpan MaxPlayTime { get; set; }

    /// <summary>
    /// Minimum number of players
    /// </summary>
    /// <example>1</example>
    [Required]
    public required byte MinPlayerCount { get; set; }

    /// <summary>
    /// Maximum number of players
    /// </summary>
    /// <example>4</example>
    [Required]
    public required byte MaxPlayerCount { get; set; }

    /// <summary>
    /// Optional BoardGameGeek id
    /// </summary>
    /// <example>169786</example>
    public int? BggId { get; set; }

    /// <summary>
    /// Categories of the board game
    /// </summary>
    public List<BoardGameCategoryVM> Categories { get; set; } = [];

    /// <summary>
    /// Optional id of the board game that this game is the expansion of
    /// </summary>
    /// <example>null</example>
    public Guid? ExpansionOfId { get; set; }

    private static readonly TimeSpan PlayTimeLowerLimit = new(0, 10, 0);
    private static readonly TimeSpan PlayTimeUpperLimit = new(24, 0, 0);
    private const byte PlayerCountLowerLimit = 1;
    private const byte PlayerCountUpperLimit = 100;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var playTimeErrors = ValidateRange(
            (nameof(MinPlayTime), MinPlayTime),
            (nameof(MaxPlayTime), MaxPlayTime),
            PlayTimeLowerLimit,
            PlayTimeUpperLimit
        );
        var playerCountErrors = ValidateRange(
            (nameof(MinPlayerCount), MinPlayerCount),
            (nameof(MaxPlayerCount), MaxPlayerCount),
            PlayerCountLowerLimit,
            PlayerCountUpperLimit
        );
        var bggIdErrors = BggId is null or > 0
            ? []
            : new ValidationResult[] { new("BggId should be positive.") };

        return [.. playTimeErrors, .. playerCountErrors, .. bggIdErrors];
    }

    /// <summary>
    /// Checks that both bounds are within the limits and that the minimum is not above the maximum
    /// </summary>
    private static IEnumerable<ValidationResult> ValidateRange<T>(
        (string Name, T Value) min,
        (string Name, T Value) max,
        T lowerLimit,
        T upperLimit
    )
        where T : IComparable<T>
    {
        foreach (var (name, value) in new[] { min, max })
        {
            if (value.CompareTo(lowerLimit) < 0 || value.CompareTo(upperLimit) > 0)
            {
                yield return new($"{name} should be in the range {lowerLimit} - {upperLimit}.");
            }
        }
        if (min.Value.CompareTo(max.Value) > 0)
        {
            yield return new($"{min.Name} should be less than {max.Name}.");
        }
    }
}

public class BoardGameCreateVM : BoardGameBaseVM
{
    /// <summary>
    /// Type of the board game
    /// </summary>
    /// <example>BASE_GAME</example>
    [Required]
    public required BoardGameTypeVM Type { get; set; } = BoardGameTypeVM.BASE_GAME;
}

/// <summary>
/// Board game update request; the type cannot be changed
/// </summary>
public class BoardGameUpdateVM : BoardGameBaseVM { }

public class BoardGameResponseVM : BoardGameCreateVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }

    /// <summary>
    /// Populated parent board game data (applicable for expansions)
    /// </summary>
    /// <example>Scythe</example>
    public BoardGameResponseVM? ExpansionOf { get; set; }

    /// <summary>
    /// Calculated play statistics; null for a nested board game (expansionOf, recommendations)
    /// </summary>
    public BoardGameStatisticsVM? Statistics { get; set; }
}

public class BoardGamesResponseVM : PaginatedResponseVM<BoardGameResponseVM>;
