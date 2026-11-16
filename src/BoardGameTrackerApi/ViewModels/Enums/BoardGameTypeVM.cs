namespace BoardGameTracker.Api.ViewModels.Enums;

/// <summary>
/// Type of board game. A Base game can have multiple expansions, and an expansion should belong to a base game.
/// </summary>
public enum BoardGameTypeVM: byte
{
    BASE_GAME,
    EXPANSION,
}
