using BoardGameTracker.Database;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public abstract class NamedEntityService<EntityType, FilterType>
    : AbstractService<EntityType, FilterType>
    where EntityType : NamedEntity
    where FilterType : AbstractFilter<EntityType>
{
    protected NamedEntityService(BoardGameContext context)
        : base(context) { }

    /// <summary>
    /// Message of the BusinessRuleException thrown for a duplicate name
    /// </summary>
    protected abstract string DuplicateNameMessage { get; }

    protected override Task ValidateCreateAsync(
        EntityType model,
        CancellationToken cancellationToken
    )
    {
        return ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
    }

    protected override Task ValidateUpdateAsync(
        EntityType existing,
        EntityType model,
        CancellationToken cancellationToken
    )
    {
        return ThrowIfNameTakenAsync(model.Name, existing.Id, cancellationToken);
    }

    private Task ThrowIfNameTakenAsync(
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken
    )
    {
        string normalizedName = name.ToLowerInvariant();
        return ThrowIfDuplicateAsync(
            e => e.NormalizedName == normalizedName,
            excludedId,
            DuplicateNameMessage,
            cancellationToken
        );
    }
}
