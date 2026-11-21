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
        await context.BoardGames.AddRangeAsync(boardGames);
        await context.Players.AddRangeAsync(GetSeededPlayers());
        await context.PlaySessions.AddRangeAsync(GetSeededPlaySessions(boardGames));
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

    public static List<PlaySession> GetSeededPlaySessions(List<BoardGame> seededBoardGames)
    {
        // Reference the seeded expansion instance, not a new copy, so it isn't inserted twice
        PlaySession session = ModelsTestData.ScytheSession;
        session.UsedExpansions.Clear();
        session.UsedExpansions.Add(
            seededBoardGames.Single(e => e.Id == ModelsTestData.ScytheFenris.Id)
        );
        return [session];
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
