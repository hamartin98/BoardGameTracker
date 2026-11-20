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
}
