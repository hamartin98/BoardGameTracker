namespace BoardGameTracker.Models.Imports;

/// <summary>
/// Outcome of an import
/// </summary>
public class ImportResult
{
    /// <summary>
    /// Whether the changes were left unsaved
    /// </summary>
    public bool DryRun { get; set; }

    public ImportCounts Players { get; set; } = new();

    public ImportCounts BoardGames { get; set; } = new();

    public ImportCounts ScoringCategories { get; set; } = new();

    public ImportCounts PlaySessions { get; set; } = new();

    /// <summary>
    /// Data that was skipped or changed while importing
    /// </summary>
    public List<string> Warnings { get; set; } = [];
}

/// <summary>
/// Number of imported records of one kind
/// </summary>
public class ImportCounts
{
    /// <summary>
    /// New entities
    /// </summary>
    public int Created { get; set; }

    /// <summary>
    /// Records linked to an existing entity with the same name or BoardGameGeek id
    /// </summary>
    public int Matched { get; set; }

    /// <summary>
    /// Records imported by an earlier import
    /// </summary>
    public int AlreadyImported { get; set; }

    /// <summary>
    /// Records that could not be imported; see the warnings
    /// </summary>
    public int Skipped { get; set; }
}
