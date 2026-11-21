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

    protected override async Task LoadRelations(Player entity, CancellationToken cancellationToken)
    {
        // Loaded so that deleting a player also soft-deletes its scores
        await Context.Entry(entity).Collection(e => e.PlayerScores).LoadAsync(cancellationToken);
    }
}
