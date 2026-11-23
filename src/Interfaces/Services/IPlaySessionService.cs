using BoardGameTracker.Models.Commands;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Interfaces.Services;

public interface IPlaySessionService
    : ICrudService<
        PlaySession,
        PlaySessionFilter,
        CreatePlaySessionCommand,
        UpdatePlaySessionCommand
    >
{
    Task<List<PlayerScore>> GetPlayerScoresAsync(Guid id, CancellationToken cancellationToken);

    Task<List<ScoringCategory>> GetScoringCategoriesAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    Task<PlaySession> FinishAsync(Guid id, int version, CancellationToken cancellationToken);
}
