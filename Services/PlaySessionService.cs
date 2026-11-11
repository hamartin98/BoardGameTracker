using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlaySessionService : IPlaySessionService
{
    public Task<PlaySession> CreateAsync(PlaySession model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PlaySession>> GetAllAsync(
        PlaySessionFilter filter,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<PlaySession> GetByIdAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PlaySession> UpdateAsync(
        PlaySession model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
