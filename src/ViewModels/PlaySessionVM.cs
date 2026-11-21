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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Duration is not null && Duration <= TimeSpan.Zero)
        {
            yield return new("Duration should be positive.");
        }
    }
}

public class PlaySessionCreateVM : PlaySessionBaseVM { }

/// <summary>
/// Request to start a play session now; the start time is set by the server and the duration cannot be set
/// </summary>
public class PlaySessionStartVM
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
    /// Type of the play session
    /// </summary>
    /// <example>COMPETITIVE</example>
    public required PlaySessionTypeVM SessionType { get; set; }

    /// <summary>
    /// Additional notes about the play session
    /// </summary>
    public string? Notes { get; set; }
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

public class PlaySessionsResponseVM : PaginatedResponseVM<PlaySessionResponseVM>;
