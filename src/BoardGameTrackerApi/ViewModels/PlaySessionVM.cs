using BoardGameTracker.Api.ViewModels.Enums;

namespace BoardGameTracker.Api.ViewModels;

public class PlaySessionBaseVM
{
    /// <summary>
    /// Id of the board game used
    /// </summary>
    public required Guid BoardGameId { get; set; }

    /// <summary>
    /// List of expansion ids used during the session
    /// </summary>
    public ICollection<Guid> UsedExpansionIds { get; set; } = [];

    /// <summary>
    /// Starting date and time of the play session
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// Total duration of the play session
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Player score ids for the play session
    /// </summary>
    public ICollection<Guid> PlayerScoreIds { get; set; } = [];

    /// <summary>
    /// Type of the play session
    /// </summary>
    public PlaySessionTypeVM SessionType { get; set; }

    /// <summary>
    /// Additional notes about the play session
    /// </summary>
    public string? Notes { get; set; }
}

public class PlaySessionCreateVM : PlaySessionBaseVM { }

public class PlaySessionUpdateVM : PlaySessionCreateVM { }

public class PlaySessionResponseVM : PlaySessionCreateVM
{
    public required ResponseMetaDataVM MetaData { get; set; }
}
