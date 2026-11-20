using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : AbstractService<Player, PlayerFilter>, IPlayerService
{
    public PlayerService(BoardGameContext context)
        : base(context) { }

    public async Task<Player> CreateAsync(Player model, CancellationToken cancellationToken)
    {
        await ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
        Player result = await CreateEntityAsync(model, cancellationToken);
        return result;
    }

    public async Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        await DeleteEntityAsync(id, version, cancellationToken);
    }

    public async Task<PaginatedResponse<Player>> GetAllAsync(
        PlayerFilter filter,
        CancellationToken cancellationToken
    )
    {
        var result = await GetAllFilteredAsync(filter, cancellationToken);
        return result;
    }

    public async Task<Player> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Player result = await FindByIdAsync(id, cancellationToken);
        return result;
    }

    public async Task<Player> UpdateAsync(
        Player model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNameTakenAsync(model.Name, id, cancellationToken);
        Player entity = await UpdateEntityAsync(id, model, version, cancellationToken);
        return entity;
    }

    private Task ThrowIfNameTakenAsync(
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken
    )
    {
        string normalizedName = name.ToLower();
        return ThrowIfDuplicateAsync(
            p => p.NormalizedName == normalizedName,
            excludedId,
            "A player with this name already exists.",
            cancellationToken
        );
    }

    protected override Task LoadRelations(Player entity, CancellationToken cancellationToken)
    {
        // Scores are not part of the player response, so nothing to load.
        return Task.CompletedTask;
    }
}
