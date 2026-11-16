using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;

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
        BoardGame entity = await UpdateEntityAsync(id, model, version, cancellationToken);
        return entity;
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
}
