using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[Route("[controller]")]
public class BoardGamesController(IMapper mapper, IBoardGameService boardGameService)
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
}
