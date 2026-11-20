using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class BoardGameService : AbstractService<BoardGame, BoardGameFilter>, IBoardGameService
{
    public BoardGameService(BoardGameContext context)
        : base(context) { }

    public override async Task<BoardGame> CreateAsync(
        BoardGame model,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
        await CheckParentAsync(model, cancellationToken);
        return await base.CreateAsync(model, cancellationToken);
    }

    public override async Task<BoardGame> UpdateAsync(
        BoardGame model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        BoardGame existing = await FindByIdAsync(id, cancellationToken);
        model.Type = existing.Type;
        if (model.Type == BoardGameType.EXPANSION && model.ExpansionOfId is null)
        {
            throw new BusinessRuleException(
                "An expansion must have the board game set that it belongs to."
            );
        }
        if (model.Type == BoardGameType.BASE_GAME && model.ExpansionOfId is not null)
        {
            throw new BusinessRuleException("A base game should not have an expansionOf relation.");
        }
        await ThrowIfNameTakenAsync(model.Name, id, cancellationToken);
        await CheckParentAsync(model, cancellationToken);
        return await base.UpdateAsync(model, id, version, cancellationToken);
    }

    private Task ThrowIfNameTakenAsync(
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken
    )
    {
        string normalizedName = name.ToLowerInvariant();
        return ThrowIfDuplicateAsync(
            bg => bg.NormalizedName == normalizedName,
            excludedId,
            "A board game with this name already exists.",
            cancellationToken
        );
    }

    private async Task CheckParentAsync(BoardGame boardGame, CancellationToken cancellationToken)
    {
        if (boardGame.ExpansionOfId is not null)
        {
            BoardGame parent = await FindByIdAsync(
                boardGame.ExpansionOfId.Value,
                cancellationToken
            );
            if (parent.Type != BoardGameType.BASE_GAME)
            {
                throw new BusinessRuleException("The expansion must belong to a base game.");
            }
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
