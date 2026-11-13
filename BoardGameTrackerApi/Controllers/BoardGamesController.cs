using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BoardGamesController : Controller
{
    private readonly IMapper Mapper;
    private readonly IBoardGameService BoardGameService;

    public BoardGamesController(IMapper mapper, IBoardGameService boardGameService)
    {
        Mapper = mapper;
        BoardGameService = boardGameService;
    }

    /// <summary>
    /// Create board game
    /// </summary>
    /// <param name="createVM">Board game create request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="201">Entity successfully created</response>
    /// <response code="500">Internal system error</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BoardGameResponseVM), StatusCodes.Status201Created)]
    [Produces("application/json")]
    public async Task<IActionResult> Create(
        [FromBody] BoardGameCreateVM createVM,
        CancellationToken cancellationToken
    )
    {
        BoardGame entity = Mapper.Map<BoardGame>(createVM);
        BoardGame result = await BoardGameService.CreateAsync(entity, cancellationToken);
        BoardGameResponseVM mappedResult = Mapper.Map<BoardGameResponseVM>(result);
        return CreatedAtAction(
            nameof(GetById),
            new { boardGameId = mappedResult.MetaData.Id },
            mappedResult
        );
    }

    /// <summary>
    /// Get board game by id
    /// </summary>
    /// <param name="boardGameId">Id of the board game to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Entity successfully retrieved</response>
    /// <response code="500">Internal system error</response>
    [HttpGet("{boardGameId}", Name = "GetById")]
    [Produces("application/json")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid boardGameId,
        CancellationToken cancellationToken
    )
    {
        BoardGame result = await BoardGameService.GetByIdAsync(boardGameId, cancellationToken);
        BoardGameResponseVM mappedResult = Mapper.Map<BoardGameResponseVM>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Get all board games
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Entities successfully retrieved</response>
    /// <response code="500">Internal system error</response>
    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var filter = new BoardGameFilter();
        List<BoardGame> result = [.. await BoardGameService.GetAllAsync(filter, cancellationToken)];
        List<BoardGameResponseVM> mappedResult = Mapper.Map<List<BoardGameResponseVM>>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Delete board game
    /// </summary>
    /// <param name="boardGameId">Id of the board game to delete</param>
    /// <param name="version">Version of the board game for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="206">Entity successfully deleted</response>
    /// <response code="500">Internal system error</response>
    [HttpDelete("{boardGameId}")]
    [Produces("application/json")]
    public async Task<IActionResult> Delete(
        Guid boardGameId,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        await BoardGameService.DeleteAsync(boardGameId, version, cancellationToken);
        return NoContent();
    }
}
