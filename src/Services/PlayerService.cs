using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;

namespace BoardGameTracker.Services;

public class PlayerService : NamedEntityService<Player, PlayerFilter>, IPlayerService
{
    public PlayerService(BoardGameContext context)
        : base(context) { }

    protected override string DuplicateNameMessage => "A player with this name already exists.";

    protected override async Task LoadDeleteRelationsAsync(
        Player entity,
        CancellationToken cancellationToken
    )
    {
        await Context.Entry(entity).Collection(e => e.PlayerScores).LoadAsync(cancellationToken);
    }
}
