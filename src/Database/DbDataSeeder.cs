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

        await context.BoardGames.AddRangeAsync(GetSeededBoardGames());
        await context.Players.AddRangeAsync(GetSeededPlayers());

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

    public static List<Player> GetSeededPlayers()
    {
        return [ModelsTestData.Alice, ModelsTestData.Bob];
    }
}
