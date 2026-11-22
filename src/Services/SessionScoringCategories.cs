using BoardGameTracker.Database;
using BoardGameTracker.Models;
using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Services;

internal static class SessionScoringCategories
{
    /// <summary>
    /// Categories scored in a session: the board game's, then each used expansion's (by expansion name), each in scoring order
    /// </summary>
    public static async Task<List<ScoringCategory>> GetSessionScoringCategoriesAsync(
        this BoardGameContext context,
        Guid sessionId,
        CancellationToken cancellationToken
    )
    {
        PlaySession session =
            await context
                .Set<PlaySession>()
                .AsNoTracking()
                .Include(e => e.UsedExpansions)
                .FirstOrDefaultAsync(e => e.Id == sessionId, cancellationToken)
            ?? throw new EntityNotFoundException<PlaySession>();

        List<Guid> boardGameIds =
        [
            session.BoardGameId,
            .. session.UsedExpansions.OrderBy(e => e.Name).ThenBy(e => e.Id).Select(e => e.Id),
        ];
        List<ScoringCategory> categories = await context
            .Set<ScoringCategory>()
            .AsNoTracking()
            .Where(e => boardGameIds.Contains(e.BoardGameId))
            .ToListAsync(cancellationToken);

        return
        [
            .. categories
                .OrderBy(e => boardGameIds.IndexOf(e.BoardGameId))
                .ThenBy(e => e.Order)
                .ThenBy(e => e.Id),
        ];
    }
}
