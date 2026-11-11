using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : IPlayerService
{
    public Task<Player> CreateAsync(Player model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Player>> GetAllAsync(
        PlayerFilter filter,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<Player> GetByIdAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Player> UpdateAsync(
        Player model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
