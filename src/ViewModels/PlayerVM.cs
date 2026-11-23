using System.ComponentModel.DataAnnotations;

namespace BoardGameTracker.ViewModels;

public class PlayerBaseVM
{
    /// <summary>
    /// Name of the player
    /// </summary>
    /// <example>Alice</example>
    [Required]
    public required string Name { get; set; }
}

public class PlayerCreateVM : PlayerBaseVM { }

public class PlayerUpdateVM : PlayerCreateVM { }

public class PlayerResponseVM : PlayerCreateVM
{
    /// <summary>
    /// Response metadata
    /// </summary>
    public required ResponseMetaDataVM MetaData { get; set; }

    /// <summary>
    /// Calculated play statistics
    /// </summary>
    public PlayerStatisticsVM? Statistics { get; set; }
}

public class PlayersResponseVM : PaginatedResponseVM<PlayerResponseVM>;
