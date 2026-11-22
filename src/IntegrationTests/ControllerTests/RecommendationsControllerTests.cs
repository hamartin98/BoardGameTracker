using System.Net;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class RecommendationsControllerTests : IntegrationTestBase
{
    private async Task<List<BoardGameRecommendationVM>> GetAsync(string query)
    {
        var response = await Client.GetAsync($"/Recommendations?{query}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await ReadAsync<List<BoardGameRecommendationVM>>(response);
    }

    private async Task<BoardGameResponseVM> CreateBoardGameAsync(BoardGameCreateVM boardGame)
    {
        return await ReadAsync<BoardGameResponseVM>(await PostAsync("/BoardGames", boardGame));
    }

    // GET /Recommendations

    [Fact]
    public async Task GetForPlayerCountReturnsSeededBaseGameTest()
    {
        var recommendations = await GetAsync("playerCount=2");

        ViewModelsTestData.CheckBoardGameRecommendationVM(
            ViewModelsTestData.ScytheRecommendationVM,
            Assert.Single(recommendations)
        );
    }

    [Fact]
    public async Task GetForUnsupportedPlayerCountReturnsEmptyListTest()
    {
        Assert.Empty(await GetAsync("playerCount=6"));
    }

    [Fact]
    public async Task RecencyGrowsWithTimeSinceLastPlayTest()
    {
        Clock.UtcNow += TimeSpan.FromDays(60);

        var recommendations = await GetAsync("playerCount=2");

        ViewModelsTestData.CheckBoardGameRecommendationVM(
            ViewModelsTestData.ScytheRecommendationAfterHalfLifeVM,
            Assert.Single(recommendations)
        );
    }

    [Fact]
    public async Task AveragePlayTimeUsesFinishedSessionsOfThePlayerCountTest()
    {
        await PostAsync("/PlaySessions", ViewModelsTestData.ScytheShortTwoPlayerSessionCreateVM);
        await PostAsync("/PlaySessions", ViewModelsTestData.ScytheSoloSessionCreateVM);
        await PostAsync("/PlaySessions", ViewModelsTestData.ScytheSessionWithoutStartTimeCreateVM);

        var twoPlayers = Assert.Single(await GetAsync("playerCount=2"));
        var solo = Assert.Single(await GetAsync("playerCount=1"));

        Assert.Equal(new TimeSpan(1, 30, 0), twoPlayers.AveragePlayTime);
        Assert.Equal(new TimeSpan(0, 45, 0), solo.AveragePlayTime);
        Assert.Equal(4, twoPlayers.SessionCount);
    }

    [Fact]
    public async Task NeverPlayedGamesComeFirstOrderedByNameTest()
    {
        var kingdomino = await CreateBoardGameAsync(ViewModelsTestData.KingdominoCreateVM);
        var azul = await CreateBoardGameAsync(ViewModelsTestData.AzulCreateVM);

        var recommendations = await GetAsync("playerCount=2");

        Assert.Equal(
            [azul.MetaData.Id, kingdomino.MetaData.Id, CommonTestData.Guid1],
            recommendations.Select(e => e.BoardGame.MetaData.Id)
        );
        Assert.All(recommendations.Take(2), e => Assert.Equal(100, e.Score));
    }

    [Fact]
    public async Task PlayTimeAndCategoriesRankFittingGamesHigherTest()
    {
        var azul = await CreateBoardGameAsync(ViewModelsTestData.AzulCreateVM);
        Clock.UtcNow += TimeSpan.FromDays(365);

        var recommendations = await GetAsync(
            "playerCount=2&minPlayTime=00:30:00&maxPlayTime=00:45:00&categories=ABSTRACT"
        );

        Assert.Equal(azul.MetaData.Id, recommendations[0].BoardGame.MetaData.Id);
        Assert.Equal(100, recommendations[0].Score);
        var scythe = recommendations[1];
        Assert.Equal(0, scythe.CategoryScore);
        Assert.True(scythe.TimeScore < 0.1);
    }

    [Fact]
    public async Task LimitCutsTheListTest()
    {
        await CreateBoardGameAsync(ViewModelsTestData.AzulCreateVM);

        var recommendations = await GetAsync("playerCount=2&limit=1");

        Assert.Single(recommendations);
    }

    [Fact]
    public async Task DefaultAndMaximumLimitTest()
    {
        for (int i = 0; i < 10; i++)
        {
            var boardGame = ViewModelsTestData.AzulCreateVM;
            boardGame.Name = $"Azul {i}";
            await CreateBoardGameAsync(boardGame);
        }

        Assert.Equal(10, (await GetAsync("playerCount=2")).Count);
        Assert.Equal(11, (await GetAsync("playerCount=2&limit=1000")).Count);
    }

    [Theory]
    [InlineData("")]
    [InlineData("playerCount=0")]
    [InlineData("playerCount=101")]
    [InlineData("playerCount=2&minPlayTime=02:00:00&maxPlayTime=01:00:00")]
    [InlineData("playerCount=2&maxPlayTime=-00:10:00")]
    [InlineData("playerCount=2&limit=0")]
    public async Task InvalidQueryReturnsBadRequestTest(string query)
    {
        var response = await Client.GetAsync($"/Recommendations?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
