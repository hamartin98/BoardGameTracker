using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : AbstractService<Player, PlayerFilter>, IPlayerService
{
    public PlayerService(BoardGameContext context)
        : base(context) { }

    protected override Task ValidateCreateAsync(Player model, CancellationToken cancellationToken)
    {
        return ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
    }

    protected override Task ValidateUpdateAsync(
        Player existing,
        Player model,
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
            p => p.NormalizedName == normalizedName,
            excludedId,
            "A player with this name already exists.",
            cancellationToken
        );
    }
}
