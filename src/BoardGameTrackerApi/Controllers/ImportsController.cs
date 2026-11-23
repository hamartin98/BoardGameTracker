using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Imports;
using BoardGameTracker.ViewModels;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImportsController(IMapper mapper, IImportService importService) : ControllerBase
{
    /// <summary>
    /// Import the players, games and plays of a BoardRecord backup; records imported before are left unchanged
    /// </summary>
    /// <param name="file">BoardRecord backup JSON file</param>
    /// <param name="dryRun">Report the result without saving it</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("board-record")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "Backup successfully imported")]
    public async Task<ActionResult<ImportResultVM>> ImportBoardRecord(
        IFormFile file,
        [FromQuery] bool dryRun,
        CancellationToken cancellationToken
    )
    {
        await using Stream stream = file.OpenReadStream();
        ImportResult result = await importService.ImportBoardRecordAsync(
            stream,
            dryRun,
            cancellationToken
        );
        return Ok(mapper.Map<ImportResultVM>(result));
    }
}
