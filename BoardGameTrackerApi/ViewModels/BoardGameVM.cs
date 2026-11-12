using BoardGameTracker.Api.ViewModels.Enums;

namespace BoardGameTracker.Api.ViewModels;

public class BoardGameBaseVM
{
    /// <summary>
    /// Name of the board game
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Minimum play time
    /// </summary>
    public required TimeSpan MinPlayTime { get; set; }

    /// <summary>
    /// Maximum playtime
    /// </summary>
    public required TimeSpan MaxPlayTime { get; set; }

    /// <summary>
    /// Minimum number of players
    /// </summary>
    public required int MinPlayerCount { get; set; }

    /// <summary>
    /// Maximum number of players
    /// </summary>
    public required int MaxPlayerCount { get; set; }

    /// <summary>
    /// Optional URL for BoardGameGeek page
    /// </summary>
    public string? BggUrl { get; set; }

    /// <summary>
    /// Type of the board game
    /// </summary>
    public BoardGameTypeVM Type { get; set; } = BoardGameTypeVM.BASE_GAME;

    /// <summary>
    /// Categories of the board game
    /// </summary>
    public List<BoardGameCategoryVM> Categories { get; set; } = [];

    /// <summary>
    /// Optional id of the board game that this game is the expansion of
    /// </summary>
    public Guid? ExpansionOfId { get; set; }
}

public class BoardGameCreateVM : BoardGameBaseVM { }

public class BoardGameUpdateVM : BoardGameCreateVM { }

public class BoardGameResponseVM : BoardGameCreateVM
{
    public required ResponseMetaDataVM MetaData { get; set; }

    public BoardGameResponseVM? ExpansionOf { get; set; }

    public List<BoardGameResponseVM> Expansions { get; set; } = [];
}
