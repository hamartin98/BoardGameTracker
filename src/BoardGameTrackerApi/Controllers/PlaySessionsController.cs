using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class PlaySessionsController(IMapper mapper, IPlaySessionService playSessionService)
    : CrudController<
        PlaySession,
        PlaySessionFilter,
        PlaySessionCreateVM,
        PlaySessionUpdateVM,
        PlaySessionResponseVM,
        PlaySessionsResponseVM,
        PlaySessionFilterVM
    >(mapper, playSessionService)
{
    /// <summary>
    /// Get all player scores of the given play session
    /// </summary>
    /// <param name="id">Play session id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/PlayerScores")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entities successfully retrieved")]
    public async Task<ActionResult<List<PlayerScoreResponseVM>>> GetPlayerScores(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await playSessionService.GetPlayerScoresAsync(id, cancellationToken);
        return Ok(Mapper.Map<List<PlayerScoreResponseVM>>(result));
    }
}
