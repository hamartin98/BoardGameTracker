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
    /// Start a play session now; the start time is set to the current UTC time
    /// </summary>
    /// <param name="startVM">Start request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("Start")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Description = "Play session successfully started"
    )]
    public async Task<ActionResult<PlaySessionResponseVM>> Start(
        [FromBody] PlaySessionStartVM startVM,
        CancellationToken cancellationToken
    )
    {
        PlaySession model = Mapper.Map<PlaySession>(startVM);
        PlaySession result = await playSessionService.StartAsync(model, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            Mapper.Map<PlaySessionResponseVM>(result)
        );
    }

    /// <summary>
    /// Finish a play session; the duration is set to the time elapsed since its start
    /// </summary>
    /// <param name="id">Id of the play session to finish</param>
    /// <param name="version">Version of the play session for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("{id}/Finish")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Play session successfully finished"
    )]
    public async Task<ActionResult<PlaySessionResponseVM>> Finish(
        [FromRoute] Guid id,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        PlaySession result = await playSessionService.FinishAsync(id, version, cancellationToken);
        return Ok(Mapper.Map<PlaySessionResponseVM>(result));
    }

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

    /// <summary>
    /// Get the scoring categories of the given play session: the board game's, then each used expansion's
    /// </summary>
    /// <param name="id">Play session id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/ScoringCategories")]
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
