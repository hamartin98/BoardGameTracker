namespace BoardGameTracker.Models.Imports;

/// <summary>
/// Import settings, bound from the ImportConfig section
/// </summary>
public class ImportOptions
{
    /// <summary>
    /// Time zone id of the local times in imported files
    /// </summary>
    public string TimeZone { get; set; } = "Europe/Budapest";
}
