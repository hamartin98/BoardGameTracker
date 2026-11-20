using System.ComponentModel.DataAnnotations;
using BoardGameTracker.Utils;
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
    /// Optional URL for BoardGameGeek page
    /// </summary>
    /// <example>https://boardgamegeek.com/boardgame/169786/scythe</example>
    public string? BggUrl { get; set; }

    /// <summary>
    /// Categories of the board game
    /// </summary>
    public List<BoardGameCategoryVM> Categories { get; set; } = [];

    /// <summary>
    /// Optional id of the board game that this game is the expansion of
    /// </summary>
    /// <example>null</example>
    public Guid? ExpansionOfId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> result = [];

        // MinPlayTime && MaxPlayTime
        {
            TimeSpan minPlayTime = new(0, 10, 0);
            TimeSpan maxPlayTime = new(24, 0, 0);

            if (MinPlayTime < minPlayTime || MinPlayTime > maxPlayTime)
            {
                result.Add(
                    new($"MinPlayTime should be in the range {minPlayTime} - {maxPlayTime}.")
                );
            }
            if (MaxPlayTime < minPlayTime || MaxPlayTime > maxPlayTime)
            {
                result.Add(
                    new($"MaxPlayTime should be in the range {minPlayTime} - {maxPlayTime}.")
                );
            }
            if (MinPlayTime > MaxPlayTime)
            {
                result.Add(new ValidationResult("MinPlayTime should be less than MaxPlayTime."));
            }
        }

        // MinPlayerCount && MaxPlayerCount
        {
            byte minPlayerCount = 1;
            byte maxPlayerCount = 100;

            if (MinPlayerCount < minPlayerCount || MinPlayerCount > maxPlayerCount)
            {
                result.Add(
                    new(
                        $"MinPlayerCount should be in the range {minPlayerCount} - {maxPlayerCount}."
                    )
                );
            }
            if (MaxPlayerCount < minPlayerCount || MaxPlayerCount > maxPlayerCount)
            {
                result.Add(
                    new(
                        $"MaxPlayerCount should be in the range {minPlayerCount} - {maxPlayerCount}."
                    )
                );
            }
            if (MinPlayerCount > MaxPlayerCount)
            {
                result.Add(
                    new ValidationResult("MinPlayerCount should be less than MaxPlayerCount.")
                );
            }
        }

        // BggUrl
        if (BggUrl is not null && !Validators.IsUrlValid(BggUrl))
        {
            result.Add(new("Invalid BggUrl."));
        }

        return result;
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
}

public class BoardGamesResponseVM : PaginatedResponseVM<BoardGameResponseVM>;
