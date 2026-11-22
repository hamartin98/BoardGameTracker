using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Filters;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

public class PlayerScoreService
    : AbstractService<PlayerScore, PlayerScoreFilter>,
        IPlayerScoreService
{
    public PlayerScoreService(BoardGameContext context)
        : base(context) { }

    protected override async Task ValidateCreateAsync(
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        await ThrowIfNotExistsAsync<Player>(model.PlayerId, cancellationToken);
        await ThrowIfNotExistsAsync<PlaySession>(model.SessionId, cancellationToken);

        Guid playerId = model.PlayerId;
        Guid sessionId = model.SessionId;
        bool isSessionPlayer = await Context
            .Set<PlaySession>()
            .AnyAsync(
                e => e.Id == sessionId && e.Players.Any(p => p.Id == playerId),
                cancellationToken
            );
        if (!isSessionPlayer)
        {
            throw new BusinessRuleException("The player is not a player of the play session.");
        }

        await ThrowIfDuplicateAsync(
            e => e.PlayerId == playerId && e.SessionId == sessionId,
            null,
            "The player already has a score in this play session.",
            cancellationToken
        );
    }

    protected override Task ValidateUpdateAsync(
        PlayerScore existing,
        PlayerScore model,
        CancellationToken cancellationToken
    )
    {
        model.PlayerId = existing.PlayerId;
        model.SessionId = existing.SessionId;
        return Task.CompletedTask;
    }
}
