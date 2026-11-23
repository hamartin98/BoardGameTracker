namespace BoardGameTracker.ViewModels;

public class ImportResultVM
{
    /// <summary>
    /// Whether the changes were left unsaved
    /// </summary>
    public required bool DryRun { get; set; }

    public required ImportCountsVM Players { get; set; }

    public required ImportCountsVM BoardGames { get; set; }

    public required ImportCountsVM ScoringCategories { get; set; }

    public required ImportCountsVM PlaySessions { get; set; }

    /// <summary>
    /// Data that was skipped or changed while importing
    /// </summary>
    public required List<string> Warnings { get; set; }
}

public class ImportCountsVM
{
    /// <summary>
    /// New entities
    /// </summary>
    public required int Created { get; set; }

    /// <summary>
    /// Records linked to an existing entity with the same name or BoardGameGeek id
    /// </summary>
    public required int Matched { get; set; }

    /// <summary>
    /// Records imported by an earlier import
    /// </summary>
    public required int AlreadyImported { get; set; }

    /// <summary>
    /// Records that could not be imported; see the warnings
    /// </summary>
    public required int Skipped { get; set; }
}
