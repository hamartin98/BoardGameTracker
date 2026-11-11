namespace BoardGameTracker.Api.ViewModels;

public class PlayerBaseVM
{
    /// <summary>
    /// Name of the player
    /// </summary>
    public required string Name { get; set; }
}

public class PlayerCreateVM : PlayerBaseVM { }

public class PlayerUpdateVM : PlayerCreateVM { }

public class PlayerResponseVM : PlayerCreateVM
{
    public required ResponseMetaDataVM MetaData { get; set; }
}
