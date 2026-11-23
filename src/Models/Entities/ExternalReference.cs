namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Link between a record of an imported external source and the entity it was imported into or matched with
/// </summary>
public class ExternalReference
{
    /// <summary>
    /// Id of the reference
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the external source (e.g. BoardRecord)
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// Id of the record in the external source
    /// </summary>
    public required Guid ExternalId { get; set; }

    /// <summary>
    /// Name of the entity type the record belongs to
    /// </summary>
    public required string EntityType { get; set; }

    /// <summary>
    /// Id of the entity the record was imported into or matched with
    /// </summary>
    public required Guid EntityId { get; set; }

    /// <summary>
    /// Time of the import
    /// </summary>
    public DateTime ImportedAt { get; set; }
}
