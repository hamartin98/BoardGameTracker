using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.Interfaces.Services;

/// <summary>
/// Fills the calculated statistics of entities from their play sessions
/// </summary>
public interface IStatisticsService
{
    Task LoadBoardGameStatisticsAsync(
        IReadOnlyCollection<BoardGame> boardGames,
        CancellationToken cancellationToken
    );

    Task LoadPlayerStatisticsAsync(
        IReadOnlyCollection<Player> players,
        CancellationToken cancellationToken
    );
}
