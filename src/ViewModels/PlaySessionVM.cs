using System.ComponentModel.DataAnnotations;
using BoardGameTracker.ViewModels.Enums;

namespace BoardGameTracker.ViewModels;

public class PlaySessionBaseVM : IValidatableObject
{
    /// <summary>
    /// Id of the base game played
    /// </summary>
    public required Guid BoardGameId { get; set; }

    /// <summary>
    /// Ids of the expansions of that game used during the session
    /// </summary>
    public ICollection<Guid> UsedExpansionIds { get; set; } = [];

    /// <summary>
    /// Ids of the players taking part in the session
    /// </summary>
    public ICollection<Guid> PlayerIds { get; set; } = [];

    /// <summary>
    /// Id of the player who starts the game; must be one of the session's players
    /// </summary>
    public Guid? StartPlayerId { get; set; }

    /// <summary>
    /// Starting date and time of the play session
    /// </summary>
    /// <example>2026-11-20T18:00:00Z</example>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Total duration of the play session
    /// </summary>
    /// <example>01:30:00</example>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Type of the play session
    /// </summary>
    /// <example>COMPETITIVE</example>
    public PlaySessionTypeVM SessionType { get; set; }

    /// <summary>
    /// Additional notes about the play session
    /// </summary>
    public string? Notes { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Duration is not null && Duration <= TimeSpan.Zero)
        {
            yield return new("Duration should be positive.");
        }
    }
}

public class PlaySessionCreateVM : PlaySessionBaseVM
{
    /// <summary>
    /// Whether to pick a random start player from the players; StartPlayerId must not be set
    /// </summary>
    public bool RandomStartPlayer { get; set; }

    /// <summary>
    /// Whether to start the session now: the server sets the start time and leaves the duration empty; StartTime and Duration must not be set
    /// </summary>
    public bool StartNow { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (ValidationResult result in base.Validate(validationContext))
        {
            yield return result;
        }
        if (RandomStartPlayer && StartPlayerId is not null)
        {
            yield return new(PlaySessionValidation.RandomAndFixedStartPlayerMessage);
        }
        if (StartNow && (StartTime is not null || Duration is not null))
        {
            yield return new(PlaySessionValidation.StartNowWithTimesMessage);
        }
    }
}

public class PlaySessionUpdateVM : PlaySessionBaseVM { }

public class PlaySessionResponseVM : PlaySessionBaseVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }

    /// <summary>
    /// Ids of the player scores recorded for the session
    /// </summary>
    public ICollection<Guid> PlayerScoreIds { get; set; } = [];
}

internal static class PlaySessionValidation
{
    public const string RandomAndFixedStartPlayerMessage =
        "StartPlayerId cannot be set when RandomStartPlayer is true.";

    public const string StartNowWithTimesMessage =
        "StartTime and Duration cannot be set when StartNow is true.";
}

public class PlaySessionsResponseVM : PaginatedResponseVM<PlaySessionResponseVM>;
