using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class BoardGameService : NamedEntityService<BoardGame, BoardGameFilter>, IBoardGameService
{
    public BoardGameService(BoardGameContext context)
        : base(context) { }

    protected override string DuplicateNameMessage => "A board game with this name already exists.";

    protected override async Task ValidateCreateAsync(
        BoardGame model,
        CancellationToken cancellationToken
    )
    {
        await base.ValidateCreateAsync(model, cancellationToken);
        await CheckExpansionRelationAsync(model, cancellationToken);
    }

    protected override async Task ValidateUpdateAsync(
        BoardGame existing,
        BoardGame model,
        CancellationToken cancellationToken
    )
    {
        model.Type = existing.Type;
        await base.ValidateUpdateAsync(existing, model, cancellationToken);
        await CheckExpansionRelationAsync(model, cancellationToken);
    }

    private async Task CheckExpansionRelationAsync(
        BoardGame boardGame,
        CancellationToken cancellationToken
    )
    {
        if (boardGame.Type == BoardGameType.BASE_GAME)
        {
            if (boardGame.ExpansionOfId is not null)
            {
                throw new BusinessRuleException(
                    "A base game should not have an expansionOf relation."
                );
            }
            return;
        }

        if (boardGame.ExpansionOfId is null)
        {
            throw new BusinessRuleException(
                "An expansion must have the board game set that it belongs to."
            );
        }

        BoardGame parent = await FindByIdAsync(boardGame.ExpansionOfId.Value, cancellationToken);
        if (parent.Type != BoardGameType.BASE_GAME)
        {
            throw new BusinessRuleException("The expansion must belong to a base game.");
        }
    }

    protected override async Task LoadRelations(
        BoardGame entity,
        CancellationToken cancellationToken
    )
    {
        var entry = Context.Entry(entity);
        if (entity.Type == BoardGameType.BASE_GAME)
        {
            await entry.Collection(e => e.Expansions).LoadAsync(cancellationToken);
        }
        else if (entity.Type == BoardGameType.EXPANSION)
        {
            await entry.Reference(e => e.ExpansionOf).LoadAsync(cancellationToken);
        }
    }

    protected override async Task LoadDeleteRelationsAsync(
        BoardGame entity,
        CancellationToken cancellationToken
    )
    {
        await LoadRelations(entity, cancellationToken);
        // Tracked categories of the game and its expansions are cascaded by the delete
        await Context
            .Set<ScoringCategory>()
            .Where(e => e.BoardGameId == entity.Id || e.BoardGame!.ExpansionOfId == entity.Id)
            .LoadAsync(cancellationToken);
        // Tracked sessions (and their scores) are cascaded by the delete
        await Context
            .Set<PlaySession>()
            .Where(e => e.BoardGameId == entity.Id)
            .Include(e => e.PlayerScores)
            .LoadAsync(cancellationToken);
    }

    public async Task<List<BoardGame>> GetExpansionsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<BoardGame>(id, cancellationToken);
        var result = await Context
            .Set<BoardGame>()
            .AsQueryable()
            .Where(bg => bg.ExpansionOfId == id && bg.Type == BoardGameType.EXPANSION)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return result;
    }
}
