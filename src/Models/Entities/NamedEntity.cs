namespace BoardGameTracker.Models.Entities;

/// <summary>
/// Entity with a name that is unique (case-insensitive) among non-deleted entities
/// </summary>
public abstract class NamedEntity : AbstractEntity
{
    /// <summary>
    /// Name of the entity
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Lowercase name, used for uniqueness and search
    /// </summary>
    public string NormalizedName
    {
        get => Name.ToLowerInvariant();
        set { }
    }
}
