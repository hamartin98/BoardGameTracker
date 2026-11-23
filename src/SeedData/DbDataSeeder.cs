using BoardGameTracker.Database;
using BoardGameTracker.Models.Entities;

namespace BoardGameTracker.SeedData;

public static class DbDataSeeder
{
    public static async Task Seed(BoardGameContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Consider database inited when there is data in a table
        if (context.BoardGames.Any())
        {
            return;
        }

        List<BoardGame> boardGames = GetSeededBoardGames();
        List<Player> players = GetSeededPlayers();
        await context.BoardGames.AddRangeAsync(boardGames);
        await context.Players.AddRangeAsync(players);
        await context.ScoringCategories.AddRangeAsync(GetSeededScoringCategories());
        await context.PlaySessions.AddRangeAsync(GetSeededPlaySessions(boardGames, players));
        await context.PlayerScores.AddRangeAsync(GetSeededPlayerScores());

        await context.SaveChangesAsync();
    }

    public static List<BoardGame> GetSeededBoardGames()
    {
        return
        [
            SeededEntities.Scythe,
            SeededEntities.ScytheFenris,
            SeededEntities.DeletedBoardGame,
        ];
    }

    public static List<PlaySession> GetSeededPlaySessions(
        List<BoardGame> seededBoardGames,
        List<Player> seededPlayers
    )
    {
        // Reference the seeded expansion and player instances, not new copies, so they aren't inserted twice
        PlaySession session = SeededEntities.ScytheSession;
        session.UsedExpansions.Clear();
        session.UsedExpansions.Add(
            seededBoardGames.Single(e => e.Id == SeededEntities.ScytheFenris.Id)
        );
        session.Players.Clear();
        foreach (Player player in seededPlayers)
        {
            session.Players.Add(player);
        }
        return [session];
    }

    public static List<ScoringCategory> GetSeededScoringCategories()
    {
        return
        [
            SeededEntities.ScytheStars,
            SeededEntities.ScytheTerritories,
            SeededEntities.FenrisBonus,
        ];
    }

    public static List<PlayerScore> GetSeededPlayerScores()
    {
        return [SeededEntities.AliceScytheScore, SeededEntities.BobScytheScore];
    }

    public static List<Player> GetSeededPlayers()
    {
        return [SeededEntities.Alice, SeededEntities.Bob];
    }
}
