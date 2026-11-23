using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Models.Commands;

/// <summary>
/// Request to update a play session: its stored values and the ids of the entities it references
/// </summary>
public class UpdatePlaySessionCommand
{
    /// <summary>
    /// Stored values of the play session
    /// </summary>
    public required PlaySession Values { get; set; }

    /// <summary>
    /// Ids of the expansions used; resolved into PlaySession.UsedExpansions
    /// </summary>
    public ICollection<Guid> UsedExpansionIds { get; set; } = [];

    /// <summary>
    /// Ids of the players taking part; resolved into PlaySession.Players
    /// </summary>
    public ICollection<Guid> PlayerIds { get; set; } = [];
}

/// <summary>
/// Request to create a play session
/// </summary>
public class CreatePlaySessionCommand : UpdatePlaySessionCommand
{
    /// <summary>
    /// Whether to pick a random start player from the players
    /// </summary>
    public bool RandomStartPlayer { get; set; }

    /// <summary>
    /// Whether to set the start time to now and clear the duration
    /// </summary>
    public bool StartNow { get; set; }
}
