using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Interfaces.Services;

public interface IScoringCategoryService : ICrudService<ScoringCategory, ScoringCategoryFilter>
{
    Task<List<ScoringCategory>> GetByBoardGameAsync(
        Guid boardGameId,
        CancellationToken cancellationToken
    );

    Task<List<ScoringCategory>> ReorderAsync(
        Guid boardGameId,
        List<Guid> orderedIds,
        CancellationToken cancellationToken
    );
}
