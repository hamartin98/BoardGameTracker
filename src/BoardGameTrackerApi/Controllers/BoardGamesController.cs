using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class BoardGamesController(
    IMapper mapper,
    IBoardGameService boardGameService,
    IScoringCategoryService scoringCategoryService
)
    : CrudController<
        BoardGame,
        BoardGameFilter,
        BoardGameCreateVM,
        BoardGameUpdateVM,
        BoardGameResponseVM,
        BoardGamesResponseVM,
        BoardGameFilterVM
    >(mapper, boardGameService)
{
    /// <summary>
    /// Get all expansions of the given board game
    /// </summary>
    /// <param name="id">Board game id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/Expansions")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entities successfully retrieved")]
    public async Task<ActionResult<List<BoardGameResponseVM>>> GetExpansions(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await boardGameService.GetExpansionsAsync(id, cancellationToken);
        return Ok(Mapper.Map<List<BoardGameResponseVM>>(result));
    }

    /// <summary>
    /// Get the scoring categories of the given board game in scoring order
    /// </summary>
    /// <param name="id">Board game id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}/ScoringCategories")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entities successfully retrieved")]
    public async Task<ActionResult<List<ScoringCategoryResponseVM>>> GetScoringCategories(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await scoringCategoryService.GetByBoardGameAsync(id, cancellationToken);
        return Ok(Mapper.Map<List<ScoringCategoryResponseVM>>(result));
    }

    /// <summary>
    /// Set the scoring order of the given board game's categories
    /// </summary>
    /// <param name="id">Board game id</param>
    /// <param name="orderedIds">Ids of every scoring category of the board game, in the new order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id}/ScoringCategories/Order")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Description = "Scoring categories successfully reordered"
    )]
    public async Task<ActionResult<List<ScoringCategoryResponseVM>>> ReorderScoringCategories(
        [FromRoute] Guid id,
        [FromBody] List<Guid> orderedIds,
        CancellationToken cancellationToken
    )
    {
        var result = await scoringCategoryService.ReorderAsync(id, orderedIds, cancellationToken);
        return Ok(Mapper.Map<List<ScoringCategoryResponseVM>>(result));
    }
}
