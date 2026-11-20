using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using BoardGameTracker.ViewModels;
using BoardGameTracker.ViewModels.Filters;
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
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(BoardGameResponseVM),
        StatusCodes.Status201Created,
        Description = "Entity successfully created"
    )]
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
    [HttpGet("{boardGameId}", Name = "GetById")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(BoardGameResponseVM),
        StatusCodes.Status200OK,
        Description = "Entity successfully retrieved"
    )]
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
    /// Get board games filtered and paginated
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(BoardGamesResponseVM),
        StatusCodes.Status200OK,
        Description = "Entities successfully retrieved"
    )]
    public async Task<IActionResult> GetAll(
        [FromQuery] BoardGameFilterVM filter,
        CancellationToken cancellationToken
    )
    {
        var filterModel = Mapper.Map<BoardGameFilter>(filter);
        var result = await BoardGameService.GetAllAsync(filterModel, cancellationToken);
        var mappedResult = Mapper.Map<BoardGamesResponseVM>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Get all expansions of the given board game
    /// </summary>
    /// <param name="boardGameId">Board game id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{boardGameId}/Expansions")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(List<BoardGameResponseVM>),
        StatusCodes.Status200OK,
        Description = "Entities successfully retrieved"
    )]
    public async Task<IActionResult> GetExpansions(
        Guid boardGameId,
        CancellationToken cancellationToken
    )
    {
        var result = await BoardGameService.GetExpansionsAsync(boardGameId, cancellationToken);
        var mappedResult = Mapper.Map<List<BoardGameResponseVM>>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Update board game
    /// </summary>
    /// <param name="boardGameId">Id of the board game to update</param>
    ///  /// <param name="updateVM">Board game update request body</param>
    /// <param name="version">Version of the board game for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{boardGameId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(BoardGameResponseVM),
        StatusCodes.Status200OK,
        Description = "Entity successfully updated"
    )]
    public async Task<IActionResult> Update(
        Guid boardGameId,
        BoardGameUpdateVM updateVM,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        BoardGame model = Mapper.Map<BoardGame>(updateVM);
        BoardGame updated = await BoardGameService.UpdateAsync(
            model,
            boardGameId,
            version,
            cancellationToken
        );
        BoardGameResponseVM mappedResult = Mapper.Map<BoardGameResponseVM>(updated);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Delete board game
    /// </summary>
    /// <param name="boardGameId">Id of the board game to delete</param>
    /// <param name="version">Version of the board game for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{boardGameId}")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent,
        Description = "Entity successfully deleted"
    )]
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
