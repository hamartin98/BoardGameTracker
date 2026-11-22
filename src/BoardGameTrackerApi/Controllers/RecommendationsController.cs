using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Recommendations;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RecommendationsController(IMapper mapper, IRecommendationService recommendationService)
    : ControllerBase
{
    /// <summary>
    /// Recommend base games for the player count, best first; play time, categories and the time since the last play affect the score
    /// </summary>
    /// <param name="filter">Recommendation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Recommendations successfully calculated"
    )]
    public async Task<ActionResult<List<BoardGameRecommendationVM>>> GetRecommendations(
        [FromQuery] RecommendationFilterVM filter,
        CancellationToken cancellationToken
    )
    {
        RecommendationRequest request = mapper.Map<RecommendationRequest>(filter);
        var result = await recommendationService.GetRecommendationsAsync(
            request,
            cancellationToken
        );
        return Ok(mapper.Map<List<BoardGameRecommendationVM>>(result));
    }
}
