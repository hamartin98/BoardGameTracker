using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[ApiController]
public abstract class CrudController<
    TEntity,
    TFilter,
    TCreateVM,
    TUpdateVM,
    TResponseVM,
    TListResponseVM,
    TFilterVM
> : ControllerBase
    where TEntity : AbstractEntity
    where TFilter : class
{
    protected readonly IMapper Mapper;
    private readonly ICrudService<TEntity, TFilter> Service;

    protected CrudController(IMapper mapper, ICrudService<TEntity, TFilter> service)
    {
        Mapper = mapper;
        Service = service;
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="createVM">Create request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Description = "Entity successfully created"
    )]
    public async Task<ActionResult<TResponseVM>> Create(
        [FromBody] TCreateVM createVM,
        CancellationToken cancellationToken
    )
    {
        TEntity entity = Mapper.Map<TEntity>(createVM!);
        TEntity result = await Service.CreateAsync(entity, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            Mapper.Map<TResponseVM>(result)
        );
    }

    /// <summary>
    /// Get an entity by id
    /// </summary>
    /// <param name="id">Id of the entity to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entity successfully retrieved")]
    public async Task<ActionResult<TResponseVM>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        TEntity result = await Service.GetByIdAsync(id, cancellationToken);
        return Ok(Mapper.Map<TResponseVM>(result));
    }

    /// <summary>
    /// Get entities filtered and paginated
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entities successfully retrieved")]
    public async Task<ActionResult<TListResponseVM>> GetAll(
        [FromQuery] TFilterVM filter,
        CancellationToken cancellationToken
    )
    {
        TFilter filterModel = Mapper.Map<TFilter>(filter!);
        var result = await Service.GetAllAsync(filterModel, cancellationToken);
        return Ok(Mapper.Map<TListResponseVM>(result));
    }

    /// <summary>
    /// Update an entity
    /// </summary>
    /// <param name="id">Id of the entity to update</param>
    /// <param name="updateVM">Update request body</param>
    /// <param name="version">Version of the entity for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Entity successfully updated")]
    public async Task<ActionResult<TResponseVM>> Update(
        [FromRoute] Guid id,
        [FromBody] TUpdateVM updateVM,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        TEntity model = Mapper.Map<TEntity>(updateVM!);
        TEntity updated = await Service.UpdateAsync(model, id, version, cancellationToken);
        return Ok(Mapper.Map<TResponseVM>(updated));
    }

    /// <summary>
    /// Delete an entity
    /// </summary>
    /// <param name="id">Id of the entity to delete</param>
    /// <param name="version">Version of the entity for concurrency check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent,
        Description = "Entity successfully deleted"
    )]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromQuery] int version,
        CancellationToken cancellationToken
    )
    {
        await Service.DeleteAsync(id, version, cancellationToken);
        return NoContent();
    }
}
