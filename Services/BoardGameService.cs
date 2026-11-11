using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class BoardGameService : IBoardGameService
{
    public Task<BoardGame> CreateAsync(BoardGame model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BoardGame>> GetAllAsync(
        BoardGameFilter filter,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public Task<BoardGame> GetByIdAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BoardGame> UpdateAsync(BoardGame model, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
