using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : AbstractService<Player, PlayerFilter>, IPlayerService
{
    public PlayerService(BoardGameContext context)
        : base(context) { }

    public override async Task<Player> CreateAsync(
        Player model,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
        return await base.CreateAsync(model, cancellationToken);
    }

    public override async Task<Player> UpdateAsync(
        Player model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNameTakenAsync(model.Name, id, cancellationToken);
        return await base.UpdateAsync(model, id, version, cancellationToken);
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
