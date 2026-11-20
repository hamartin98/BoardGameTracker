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
public class PlayersController : Controller
{
    private readonly IMapper Mapper;
    private readonly IPlayerService PlayerService;

    public PlayersController(IMapper mapper, IPlayerService playerService)
    {
        Mapper = mapper;
        PlayerService = playerService;
    }

    /// <summary>
    /// Create player
    /// </summary>
    /// <param name="createVM">Player create request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(PlayerResponseVM),
        StatusCodes.Status201Created,
        Description = "Entity successfully created"
    )]
    public async Task<IActionResult> Create(
        [FromBody] PlayerCreateVM createVM,
        CancellationToken cancellationToken
    )
    {
        Player entity = Mapper.Map<Player>(createVM);
        Player result = await PlayerService.CreateAsync(entity, cancellationToken);
        PlayerResponseVM mappedResult = Mapper.Map<PlayerResponseVM>(result);
        return CreatedAtAction(
            nameof(GetById),
            new { playerId = mappedResult.MetaData.Id },
            mappedResult
        );
    }

    /// <summary>
    /// Get player by id
    /// </summary>
    /// <param name="playerId">Id of the player to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{playerId}", Name = "GetPlayerById")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(PlayerResponseVM),
        StatusCodes.Status200OK,
        Description = "Entity successfully retrieved"
    )]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid playerId,
        CancellationToken cancellationToken
    )
    {
        Player result = await PlayerService.GetByIdAsync(playerId, cancellationToken);
        PlayerResponseVM mappedResult = Mapper.Map<PlayerResponseVM>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Get players filtered and paginated
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(PlayersResponseVM),
        StatusCodes.Status200OK,
        Description = "Entities successfully retrieved"
    )]
    public async Task<IActionResult> GetAll(
        [FromQuery] PlayerFilterVM filter,
        CancellationToken cancellationToken
    )
    {
        var filterModel = Mapper.Map<PlayerFilter>(filter);
        var result = await PlayerService.GetAllAsync(filterModel, cancellationToken);
        var mappedResult = Mapper.Map<PlayersResponseVM>(result);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Update player
    /// </summary>
    /// <param name="playerId">Id of the player to update</param>
    /// <param name="updateVM">Player update request body</param>
    /// <param name="version">Version of the player for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{playerId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        typeof(PlayerResponseVM),
        StatusCodes.Status200OK,
        Description = "Entity successfully updated"
    )]
    public async Task<IActionResult> Update(
        Guid playerId,
        PlayerUpdateVM updateVM,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        Player model = Mapper.Map<Player>(updateVM);
        Player updated = await PlayerService.UpdateAsync(
            model,
            playerId,
            version,
            cancellationToken
        );
        PlayerResponseVM mappedResult = Mapper.Map<PlayerResponseVM>(updated);
        return Ok(mappedResult);
    }

    /// <summary>
    /// Delete player
    /// </summary>
    /// <param name="playerId">Id of the player to delete</param>
    /// <param name="version">Version of the player for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{playerId}")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent,
        Description = "Entity successfully deleted"
    )]
    public async Task<IActionResult> Delete(
        Guid playerId,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        await PlayerService.DeleteAsync(playerId, version, cancellationToken);
        return NoContent();
    }
}
