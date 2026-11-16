namespace BoardGameTracker.Models.Enums;

/// <summary>
/// Type of board game. A Base game can have multiple expansions, and an expansion should belong to a base game.
/// </summary>
public enum BoardGameType: byte
{
    BASE_GAME,
    EXPANSION,
}
