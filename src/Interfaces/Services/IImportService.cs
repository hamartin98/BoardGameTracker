using BoardGameTracker.Models.Imports;

namespace BoardGameTracker.Interfaces.Services;

public interface IImportService
{
    /// <summary>
    /// Imports the players, games and plays of a BoardRecord backup that were not imported before
    /// </summary>
    Task<ImportResult> ImportBoardRecordAsync(
        Stream backup,
        bool dryRun,
        CancellationToken cancellationToken
    );
}
