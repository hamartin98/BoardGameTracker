using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class BoardGameService : AbstractService<BoardGame>, IBoardGameService
{
    private readonly BoardGameContext Context;

    public BoardGameService(BoardGameContext context)
        : base(context)
    {
        Context = context;
    }

    public async Task<BoardGame> CreateAsync(BoardGame model, CancellationToken cancellationToken)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(
            cancellationToken
        );

        var result = await Context.BoardGames.AddAsync(model, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result.Entity;
    }

    public async Task DeleteAsync(Guid id, int version, CancellationToken cancellationToken)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(
            cancellationToken
        );

        var entity =
            await Context
                .BoardGames.Include(e => e.Expansions)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException<BoardGame>();
        await DeleteEntity(entity, version, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IEnumerable<BoardGame>> GetAllAsync(
        BoardGameFilter filter,
        CancellationToken cancellationToken
    )
    {
        List<BoardGame> boardGames = await Context.BoardGames.ToListAsync(cancellationToken);
        return boardGames;
    }

    public async Task<BoardGame> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        BoardGame? result =
            await Context.BoardGames.FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new EntityNotFoundException<BoardGame>();

        await LoadRelations(result);
        return result;
    }

    public Task<BoardGame> UpdateAsync(
        BoardGame model,
        Guid id,
        int version,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    private async Task LoadRelations(BoardGame boardGame)
    {
        var entry = Context.Entry(boardGame);
        if (boardGame.Type == BoardGameType.BASE_GAME)
        {
            await entry.Collection(e => e.Expansions).LoadAsync();
        }
        else if (boardGame.Type == BoardGameType.EXPANSION)
        {
            await entry.Reference(e => e.ExpansionOf).LoadAsync();
        }
    }
}
