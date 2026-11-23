using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : NamedEntityService<Player, PlayerFilter>, IPlayerService
{
    private readonly IStatisticsService StatisticsService;

    public PlayerService(BoardGameContext context, IStatisticsService statisticsService)
        : base(context)
    {
        StatisticsService = statisticsService;
    }

    protected override string DuplicateNameMessage => "A player with this name already exists.";

    protected override Task LoadCalculatedValuesAsync(
        IReadOnlyCollection<Player> entities,
        CancellationToken cancellationToken
    )
    {
        return StatisticsService.LoadPlayerStatisticsAsync(entities, cancellationToken);
    }

    protected override async Task LoadDeleteRelationsAsync(
        Player entity,
        CancellationToken cancellationToken
    )
    {
        await Context.Entry(entity).Collection(e => e.PlayerScores).LoadAsync(cancellationToken);
    }
}
