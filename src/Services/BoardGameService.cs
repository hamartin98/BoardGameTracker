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
    private readonly BoardGameContext Context;

    public BoardGameService(BoardGameContext context)
        : base(context)
    {
        Context = context;
    }

    public async Task<BoardGame> CreateAsync(BoardGame model, CancellationToken cancellationToken)
    {
        await ThrowIfNameTakenAsync(model.Name, null, cancellationToken);
        await CheckParentAsync(model, cancellationToken);
        BoardGame result = await CreateEntityAsync(model, cancellationToken);
        return result;
    }

    public async Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        await DeleteEntityAsync(id, version, cancellationToken);
    }

    public async Task<PaginatedResponse<BoardGame>> GetAllAsync(
        BoardGameFilter filter,
        CancellationToken cancellationToken
    )
    {
        var result = await GetAllFilteredAsync(filter, cancellationToken);
        return result;
    }

    public async Task<BoardGame> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        BoardGame result = await FindByIdAsync(id, cancellationToken);
        await LoadRelations(result, cancellationToken);
        return result;
    }

    public async Task<BoardGame> UpdateAsync(
        BoardGame model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNameTakenAsync(model.Name, id, cancellationToken);
        await CheckParentAsync(model, cancellationToken);
        BoardGame entity = await UpdateEntityAsync(id, model, version, cancellationToken);
        return entity;
    }

    private Task ThrowIfNameTakenAsync(
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken
    )
    {
        string normalizedName = name.ToLower();
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
        await ThrowIfNotExists<BoardGame>(id, cancellationToken);
        var result = await Context
            .Set<BoardGame>()
            .AsQueryable()
            .Where(bg => bg.ExpansionOfId == id && bg.Type == BoardGameType.EXPANSION)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return result;
    }
}
