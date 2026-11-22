using BoardGameTracker.Models.Entities;
using BoardGameTracker.TestData.Models;

namespace BoardGameTracker.Database;

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
            ModelsTestData.Scythe,
            ModelsTestData.ScytheFenris,
            ModelsTestData.DeletedBoardGame,
        ];
    }

    public static List<PlaySession> GetSeededPlaySessions(
        List<BoardGame> seededBoardGames,
        List<Player> seededPlayers
    )
    {
        // Reference the seeded expansion and player instances, not new copies, so they aren't inserted twice
        PlaySession session = ModelsTestData.ScytheSession;
        session.UsedExpansions.Clear();
        session.UsedExpansions.Add(
            seededBoardGames.Single(e => e.Id == ModelsTestData.ScytheFenris.Id)
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
            ModelsTestData.ScytheStars,
            ModelsTestData.ScytheTerritories,
            ModelsTestData.FenrisBonus,
        ];
    }

    public static List<PlayerScore> GetSeededPlayerScores()
    {
        return [ModelsTestData.AliceScytheScore, ModelsTestData.BobScytheScore];
    }

    public static List<Player> GetSeededPlayers()
    {
        return [ModelsTestData.Alice, ModelsTestData.Bob];
    }
}
