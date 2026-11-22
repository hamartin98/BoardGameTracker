using BoardGameTracker.Models.Recommendations;

namespace BoardGameTracker.Interfaces.Services;

public interface IRecommendationService
{
    Task<List<BoardGameRecommendation>> GetRecommendationsAsync(
        RecommendationRequest request,
        CancellationToken cancellationToken
    );
}
