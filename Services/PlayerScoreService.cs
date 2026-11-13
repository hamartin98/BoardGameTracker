using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerScoreService : IPlayerScoreService
{
    public Task<PlayerScore> CreateAsync(PlayerScore model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PlayerScore>> GetAllAsync(
        PlayerScoreFilter filter,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<PlayerScore> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PlayerScore> UpdateAsync(
        PlayerScore model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
