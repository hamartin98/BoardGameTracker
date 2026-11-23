using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

/// <summary>
/// Statistics calculated from the seeded competitive Scythe session with Fenris: Alice (start player) won 78 to Bob's 64 in 2 hours
/// </summary>
public class StatisticsTests : IntegrationTestBase
{
    private static void CheckSeededScytheStatistics(BoardGameStatisticsVM? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(1, actual.PlayCount);
        Assert.Equal(CommonTestData.DateTime, actual.LastPlayedAt);
        Assert.Equal(TimeSpan.FromHours(2), actual.TotalPlayTime);
        var playTime = Assert.Single(actual.AveragePlayTimes);
        Assert.Equal(2, playTime.PlayerCount);
        Assert.Equal(TimeSpan.FromHours(2), playTime.AveragePlayTime);
        Assert.Equal(1, playTime.SessionCount);
        Assert.Collection(
            actual.PlayerWinRates,
            e =>
            {
                Assert.Equal(CommonTestData.Guid1, e.PlayerId);
                Assert.Equal("Alice", e.PlayerName);
                Assert.Equal(1, e.PlayCount);
                Assert.Equal(1, e.WinCount);
                Assert.Equal(1, e.WinRate);
            },
            e =>
            {
                Assert.Equal(CommonTestData.Guid2, e.PlayerId);
                Assert.Equal("Bob", e.PlayerName);
                Assert.Equal(0, e.WinRate);
            }
        );
        Assert.NotNull(actual.HighScore);
        Assert.Equal("Alice", actual.HighScore.PlayerName);
        Assert.Equal(78, actual.HighScore.Score);
        Assert.Equal(CommonTestData.Guid1, actual.HighScore.SessionId);
        Assert.Equal(71, actual.AverageScore);
        Assert.Equal(78, actual.AverageWinningScore);
        Assert.Equal(1, actual.StartPlayerWinRate);
        Assert.Null(actual.CooperativeWinRate);
    }

    [Fact]
    public async Task GetBoardGameReturnsStatisticsTest()
    {
        var boardGame = await ReadAsync<BoardGameResponseVM>(
            await Client.GetAsync($"/board-games/{CommonTestData.Guid1}")
        );

        CheckSeededScytheStatistics(boardGame.Statistics);
    }

    [Fact]
    public async Task ExpansionStatisticsComeFromSessionsUsingItTest()
    {
        var expansion = await ReadAsync<BoardGameResponseVM>(
            await Client.GetAsync($"/board-games/{CommonTestData.Guid2}")
        );

        CheckSeededScytheStatistics(expansion.Statistics);
        Assert.Null(expansion.ExpansionOf?.Statistics);
    }

    [Fact]
    public async Task GetAllBoardGamesReturnsStatisticsTest()
    {
        var page = await ReadAsync<BoardGamesResponseVM>(await Client.GetAsync("/board-games"));

        Assert.All(page.Items, e => CheckSeededScytheStatistics(e.Statistics));
    }

    [Fact]
    public async Task CreatedBoardGameHasEmptyStatisticsTest()
    {
        var created = await ReadAsync<BoardGameResponseVM>(
            await PostAsync("/board-games", ViewModelsTestData.AzulVM)
        );

        Assert.NotNull(created.Statistics);
        Assert.Equal(0, created.Statistics.PlayCount);
        Assert.Null(created.Statistics.LastPlayedAt);
        Assert.Empty(created.Statistics.PlayerWinRates);
        Assert.Null(created.Statistics.HighScore);
    }

    [Fact]
    public async Task GetPlayerReturnsStatisticsTest()
    {
        var alice = await ReadAsync<PlayerResponseVM>(
            await Client.GetAsync($"/players/{CommonTestData.Guid1}")
        );

        var actual = alice.Statistics;
        Assert.NotNull(actual);
        Assert.Equal(1, actual.PlayCount);
        Assert.Equal(CommonTestData.DateTime, actual.LastPlayedAt);
        Assert.Equal(TimeSpan.FromHours(2), actual.TotalPlayTime);
        Assert.Equal(1, actual.CompetitivePlayCount);
        Assert.Equal(1, actual.WinCount);
        Assert.Equal(1, actual.WinRate);
        Assert.NotNull(actual.MostPlayedGame);
        Assert.Equal(CommonTestData.Guid1, actual.MostPlayedGame.BoardGameId);
        Assert.Equal("Scythe", actual.MostPlayedGame.BoardGameName);
        Assert.Equal(1, actual.MostPlayedGame.PlayCount);
        var best = Assert.Single(actual.PersonalBests);
        Assert.Equal("Scythe", best.BoardGameName);
        Assert.Equal(78, best.Score);
    }

    [Fact]
    public async Task GetAllPlayersReturnsStatisticsTest()
    {
        var page = await ReadAsync<PlayersResponseVM>(await Client.GetAsync("/players"));

        var bob = page.Items.Single(e => e.Name == "Bob").Statistics;
        Assert.NotNull(bob);
        Assert.Equal(1, bob.PlayCount);
        Assert.Equal(0, bob.WinCount);
        Assert.Equal(0, bob.WinRate);
        Assert.Equal(64, Assert.Single(bob.PersonalBests).Score);
    }

    [Fact]
    public async Task StatisticsDropDeletedSessionsTest()
    {
        await DeleteAsync($"/play-sessions/{CommonTestData.Guid1}", 1);

        var boardGame = await ReadAsync<BoardGameResponseVM>(
            await Client.GetAsync($"/board-games/{CommonTestData.Guid1}")
        );

        Assert.NotNull(boardGame.Statistics);
        Assert.Equal(0, boardGame.Statistics.PlayCount);
        Assert.Empty(boardGame.Statistics.PlayerWinRates);
    }
}
