using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Commands;
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
        CreatePlaySessionCommand,
        UpdatePlaySessionCommand,
        PlaySessionCreateVM,
        PlaySessionUpdateVM,
        PlaySessionResponseVM,
        PlaySessionsResponseVM,
        PlaySessionFilterVM
    >(mapper, playSessionService)
{
    /// <summary>
    /// Finish a play session; the duration is set to the time elapsed since its start
    /// </summary>
    /// <param name="id">Id of the play session to finish</param>
    /// <param name="ifMatch">ETag of the play session for the concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("{id:guid}/finish")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Play session successfully finished"
    )]
    public async Task<ActionResult<PlaySessionResponseVM>> Finish(
        [FromRoute] Guid id,
        [FromHeader(Name = "If-Match")] string? ifMatch,
        CancellationToken cancellationToken
    )
    {
        int version = EntityTags.ParseIfMatch(ifMatch);
        PlaySession result = await playSessionService.FinishAsync(id, version, cancellationToken);
        Response.SetETag(result);
        return Ok(Mapper.Map<PlaySessionResponseVM>(result));
    }

    /// <summary>
    /// Get all player scores of the given play session
    /// </summary>
    /// <param name="id">Play session id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}/player-scores")]
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

    /// <summary>
    /// Get the scoring categories of the given play session: the board game's, then each used expansion's
    /// </summary>
    /// <param name="id">Play session id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}/scoring-categories")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entities successfully retrieved")]
    public async Task<ActionResult<List<ScoringCategoryResponseVM>>> GetScoringCategories(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await playSessionService.GetScoringCategoriesAsync(id, cancellationToken);
        return Ok(Mapper.Map<List<ScoringCategoryResponseVM>>(result));
    }
}
