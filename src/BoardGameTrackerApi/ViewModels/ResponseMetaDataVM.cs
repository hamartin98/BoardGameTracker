namespace BoardGameTracker.Api.ViewModels;

/// <summary>
/// Response metadata for common information about entities.
/// </summary>
public class ResponseMetaDataVM
{
    /// <summary>
    /// Id of the entity
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creation time of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update time of the entity
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Current version of the entity
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Whether the entity is soft-deleted
    /// </summary>
    public bool IsDeleted { get; set; }
}
