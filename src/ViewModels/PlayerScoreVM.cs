namespace BoardGameTracker.ViewModels;

public class PlayerScoreBaseVM
{
    /// <summary>
    /// Score of the player in the given session (null when no scoring is applicable)
    /// </summary>
    /// <example>78</example>
    public int? Score { get; set; }

    /// <summary>
    /// Whether the player won the game
    /// </summary>
    /// <example>true</example>
    public bool Win { get; set; }

    /// <summary>
    /// Additional information about the score entry (e.g. player color, asymmetric player info)
    /// </summary>
    /// <example>Polania</example>
    public string? Notes { get; set; }
}

public class PlayerScoreCreateVM : PlayerScoreBaseVM
{
    /// <summary>
    /// Id of the player the score belongs to
    /// </summary>
    public required Guid PlayerId { get; set; }

    /// <summary>
    /// Id of the play session the score belongs to
    /// </summary>
    public required Guid SessionId { get; set; }
}

/// <summary>
/// Player score update request; the player and session cannot be changed
/// </summary>
public class PlayerScoreUpdateVM : PlayerScoreBaseVM { }

public class PlayerScoreResponseVM : PlayerScoreCreateVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }
}

public class PlayerScoresResponseVM : PaginatedResponseVM<PlayerScoreResponseVM>;
