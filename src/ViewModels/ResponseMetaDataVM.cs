namespace BoardGameTracker.ViewModels;

/// <summary>
/// Response metadata for common information about entities.
/// </summary>
public class ResponseMetaDataVM
{
    /// <summary>
    /// Id of the entity
    /// </summary>
    /// <example>77934a16-a70a-4ac3-9c0e-000000000001</example> 
    public Guid Id { get; set; }

    /// <summary>
    /// Creation time of the entity
    /// </summary>
    /// <example>2026-11-17T12:50:47.680Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update time of the entity
    /// </summary>
    /// <example>2026-11-17T12:50:47.680Z</example>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Current version of the entity
    /// </summary>
    /// <example>1</example>
    public int Version { get; set; }

    /// <summary>
    /// Whether the entity is soft-deleted
    /// </summary>
    /// <example>false</example>
    public bool IsDeleted { get; set; }
}
