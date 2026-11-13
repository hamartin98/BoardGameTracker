namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Abstract class for common information about entities.
/// </summary>
public abstract class AbstractEntity
{
    /// <summary>
    /// Id of the entity
    /// </summary>
    public Guid Id { get; set; } = new Guid();

    /// <summary>
    /// Creation time of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update time of the entity
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current version of the entity
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Whether the entity is soft-deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
