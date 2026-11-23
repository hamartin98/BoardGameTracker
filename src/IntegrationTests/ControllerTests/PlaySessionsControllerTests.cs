using System.Net;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class PlaySessionsControllerTests : IntegrationTestBase
{
    private static readonly string ScytheSessionUrl = $"/play-sessions/{CommonTestData.Guid1}";
    private static readonly string UnknownUrl = $"/play-sessions/{Guid.Empty}";

    // GET /play-sessions

    [Fact]
    public async Task GetAllReturnsSeededPlaySessionTest()
    {
        var response = await Client.GetAsync("/play-sessions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ReadAsync<PlaySessionsResponseVM>(response);
        Assert.Equal(1, page.TotalCount);
        ViewModelsTestData.CheckPlaySessionResponseVM(
            ViewModelsTestData.ScytheSessionWithScoresVM,
            Assert.Single(page.Items)
        );
    }

    public static TheoryData<string, int> FilterCases =>
        new()
        {
            { $"boardGameId={CommonTestData.Guid1}", 1 },
            { $"boardGameId={CommonTestData.Guid2}", 0 },
            { "sessionTypes=COMPETITIVE&sessionTypes=SINGLE_PLAYER", 1 },
            { "sessionTypes=COOPERATIVE", 0 },
            {
                $"startTimeFrom={Iso(CommonTestData.DateTime)}&startTimeTo={Iso(CommonTestData.DateTime)}",
                1
            },
            { $"startTimeFrom={Iso(CommonTestData.DateTime.AddSeconds(1))}", 0 },
            { $"startTimeTo={Iso(CommonTestData.DateTime.AddSeconds(-1))}", 0 },
            { $"createdAtTo={Iso(CommonTestData.DateTime.AddSeconds(-1))}", 0 },
        };

    [Theory]
    [MemberData(nameof(FilterCases))]
    public async Task GetAllFiltersTest(string query, int expectedCount)
    {
        var response = await Client.GetAsync($"/play-sessions?{query}");

        var page = await ReadAsync<PlaySessionsResponseVM>(response);
        Assert.Equal(expectedCount, page.TotalCount);
        Assert.Equal(expectedCount, page.Items.Count);
    }

    [Fact]
    public async Task GetAllPagesTest()
    {
        Clock.UtcNow += TimeSpan.FromMinutes(1);
        await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithoutExpansionsCreateVM
        );

        var response = await Client.GetAsync("/play-sessions?page=1&pageSize=1&orderBy=createdAt");

        var page = await ReadAsync<PlaySessionsResponseVM>(response);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1, page.PageSize);
        ViewModelsTestData.CheckPlaySessionResponseVM(
            ViewModelsTestData.ScytheSessionWithScoresVM,
            Assert.Single(page.Items)
        );
    }

    [Fact]
    public async Task GetAllWithUnknownOrderByReturnsBadRequestTest()
    {
        var response = await Client.GetAsync("/play-sessions?orderBy=nope");

        await AssertProblemAsync(response, 400, "InvalidOrderByException");
    }

    // GET /play-sessions/{id}

    [Fact]
    public async Task GetByIdReturnsPlaySessionTest()
    {
        var response = await Client.GetAsync(ScytheSessionUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckPlaySessionResponseVM(
            ViewModelsTestData.ScytheSessionWithScoresVM,
            await ReadAsync<PlaySessionResponseVM>(response)
        );
    }

    [Fact]
    public async Task GetByIdOfUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await Client.GetAsync(UnknownUrl);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // GET /play-sessions/{id}/player-scores

    [Fact]
    public async Task GetPlayerScoresTest()
    {
        var response = await Client.GetAsync($"{ScytheSessionUrl}/player-scores");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var scores = await ReadAsync<List<PlayerScoreResponseVM>>(response);
        Assert.Equal(2, scores.Count);
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.AliceScytheScoreVM,
            scores[0]
        );
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.BobScytheScoreVM,
            scores[1]
        );
    }

    [Fact]
    public async Task GetPlayerScoresOfUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await Client.GetAsync($"{UnknownUrl}/player-scores");

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // GET /play-sessions/{id}/scoring-categories

    [Fact]
    public async Task GetScoringCategoriesTest()
    {
        var response = await Client.GetAsync($"{ScytheSessionUrl}/scoring-categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckScoringCategoryResponseVMs(
            [
                ViewModelsTestData.ScytheStarsVM,
                ViewModelsTestData.ScytheTerritoriesVM,
                ViewModelsTestData.FenrisBonusVM,
            ],
            await ReadAsync<List<ScoringCategoryResponseVM>>(response)
        );
    }

    [Fact]
    public async Task GetScoringCategoriesOfUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await Client.GetAsync($"{UnknownUrl}/scoring-categories");

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // POST /play-sessions

    [Fact]
    public async Task CreateTest()
    {
        var response = await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionCreateVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<PlaySessionResponseVM>(response);
        ViewModelsTestData.CheckPlaySessionVM(ViewModelsTestData.ScytheSessionCreateVM, created);
        Assert.Equal(1, created.MetaData.Version);
        Assert.Empty(created.PlayerScoreIds);

        Assert.NotNull(response.Headers.Location);
        var fetched = await ReadAsync<PlaySessionResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckPlaySessionResponseVM(created, fetched);
    }

    [Fact]
    public async Task CreateWithNegativeDurationReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionNegativeDurationCreateVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateForExpansionReturnsUnprocessableTest()
    {
        var response = await PostAsync("/play-sessions", ViewModelsTestData.FenrisSessionCreateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateForDeletedBoardGameReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.DeletedBoardGameSessionCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task CreateWithBaseGameAsExpansionReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithBaseGameAsExpansionCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateWithDeletedExpansionReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithDeletedExpansionCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task CreateWithDeletedPlayerReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithDeletedPlayerCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task CreateWithStartPlayerNotPlayingReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithStartPlayerNotPlayingCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateWithRandomStartPlayerTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionRandomStartPlayerCreateVM
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<PlaySessionResponseVM>(response);
        Assert.NotNull(created.StartPlayerId);
        Assert.Contains(created.StartPlayerId.Value, created.PlayerIds);
    }

    [Fact]
    public async Task CreateWithRandomAndFixedStartPlayerReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionRandomAndFixedStartPlayerCreateVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWithRandomStartPlayerWithoutPlayersReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionRandomStartPlayerWithoutPlayersCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    // POST /play-sessions with startNow

    [Fact]
    public async Task StartTest()
    {
        var response = await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionStartVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var started = await ReadAsync<PlaySessionResponseVM>(response);
        ViewModelsTestData.CheckPlaySessionVM(ViewModelsTestData.ScytheSessionStartedVM, started);
        Assert.Equal(1, started.MetaData.Version);

        Assert.NotNull(response.Headers.Location);
        var fetched = await ReadAsync<PlaySessionResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckPlaySessionResponseVM(started, fetched);
    }

    [Fact]
    public async Task StartNowWithStartTimeReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionStartNowWithStartTimeVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StartForExpansionReturnsUnprocessableTest()
    {
        var response = await PostAsync("/play-sessions", ViewModelsTestData.FenrisSessionStartVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task StartWithRandomStartPlayerTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionRandomStartPlayerStartVM
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var started = await ReadAsync<PlaySessionResponseVM>(response);
        Assert.NotNull(started.StartPlayerId);
        Assert.Contains(started.StartPlayerId.Value, started.PlayerIds);
    }

    [Fact]
    public async Task StartWithRandomAndFixedStartPlayerReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionRandomAndFixedStartPlayerStartVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StartWithStartPlayerNotPlayingReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/play-sessions",
            ViewModelsTestData.ScytheSessionWithStartPlayerNotPlayingStartVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    // POST /play-sessions/{id}/finish

    [Fact]
    public async Task FinishTest()
    {
        var started = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionStartVM)
        );
        string url = $"/play-sessions/{started.MetaData.Id}";
        Clock.UtcNow += TimeSpan.FromMinutes(90);

        var response = await PostEmptyAsync($"{url}/finish", 1);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var finished = await ReadAsync<PlaySessionResponseVM>(response);
        ViewModelsTestData.CheckPlaySessionVM(ViewModelsTestData.ScytheSessionFinishedVM, finished);
        Assert.Equal(2, finished.MetaData.Version);
        Assert.Equal("\"2\"", response.Headers.ETag?.Tag);

        var fetched = await ReadAsync<PlaySessionResponseVM>(await Client.GetAsync(url));
        ViewModelsTestData.CheckPlaySessionResponseVM(finished, fetched);
    }

    [Fact]
    public async Task FinishAlreadyFinishedSessionReturnsUnprocessableTest()
    {
        var response = await PostEmptyAsync($"{ScytheSessionUrl}/finish", 1);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task FinishSessionWithoutStartTimeReturnsUnprocessableTest()
    {
        var created = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync(
                "/play-sessions",
                ViewModelsTestData.ScytheSessionWithoutStartTimeCreateVM
            )
        );

        var response = await PostEmptyAsync($"/play-sessions/{created.MetaData.Id}/finish", 1);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task FinishSessionNotStartedYetReturnsUnprocessableTest()
    {
        var started = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionStartVM)
        );

        var response = await PostEmptyAsync($"/play-sessions/{started.MetaData.Id}/finish", 1);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task FinishWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await PostEmptyAsync($"{ScytheSessionUrl}/finish", 2);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task FinishWithoutIfMatchReturnsPreconditionRequiredTest()
    {
        var response = await Client.PostAsync($"{ScytheSessionUrl}/finish", null);

        await AssertProblemAsync(response, 428, "PreconditionRequiredException");
    }

    [Fact]
    public async Task FinishUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await PostEmptyAsync($"{UnknownUrl}/finish", 1);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // PUT /play-sessions/{id}

    [Fact]
    public async Task UpdateTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionRematchUpdateVM
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<PlaySessionResponseVM>(response);
        ViewModelsTestData.CheckPlaySessionVM(
            ViewModelsTestData.ScytheSessionRematchUpdateVM,
            updated
        );
        Assert.Equal(CommonTestData.Guid1, updated.MetaData.Id);
        Assert.Equal(2, updated.MetaData.Version);

        var fetched = await ReadAsync<PlaySessionResponseVM>(
            await Client.GetAsync(ScytheSessionUrl)
        );
        ViewModelsTestData.CheckPlaySessionResponseVM(updated, fetched);
    }

    [Fact]
    public async Task UpdateOnlyExpansionsBumpsVersionTest()
    {
        var created = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionCreateVM)
        );

        var response = await PutAsync(
            $"/play-sessions/{created.MetaData.Id}",
            1,
            ViewModelsTestData.ScytheSessionUpdateVM
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<PlaySessionResponseVM>(response);
        Assert.Empty(updated.UsedExpansionIds);
        Assert.Equal(2, updated.MetaData.Version);
    }

    [Fact]
    public async Task UpdateOnlyPlayersBumpsVersionTest()
    {
        var created = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync("/play-sessions", ViewModelsTestData.ScytheSessionCreateVM)
        );

        var response = await PutAsync(
            $"/play-sessions/{created.MetaData.Id}",
            1,
            ViewModelsTestData.ScytheSessionWithoutPlayersUpdateVM
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<PlaySessionResponseVM>(response);
        Assert.Empty(updated.PlayerIds);
        Assert.Null(updated.StartPlayerId);
        Assert.Equal(2, updated.MetaData.Version);
    }

    [Fact]
    public async Task UpdateRemovingScoredExpansionReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateRemovingPlayerWithScoreReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionWithoutAliceUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateWithDeletedPlayerReturnsNotFoundTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionWithDeletedPlayerUpdateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task UpdateWithStartPlayerNotPlayingReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionWithStartPlayerNotPlayingUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            2,
            ViewModelsTestData.ScytheSessionRematchUpdateVM
        );

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task UpdateUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await PutAsync(
            UnknownUrl,
            1,
            ViewModelsTestData.ScytheSessionRematchUpdateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task UpdateToExpansionReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.FenrisSessionUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateWithDeletedExpansionReturnsNotFoundTest()
    {
        var response = await PutAsync(
            ScytheSessionUrl,
            1,
            ViewModelsTestData.ScytheSessionWithDeletedExpansionUpdateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // DELETE /play-sessions/{id}

    [Fact]
    public async Task DeleteCascadesToScoresTest()
    {
        var response = await DeleteAsync(ScytheSessionUrl, 1);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(ScytheSessionUrl)).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Client.GetAsync($"/player-scores/{CommonTestData.Guid1}")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Client.GetAsync($"/player-scores/{CommonTestData.Guid2}")).StatusCode
        );
    }

    [Fact]
    public async Task DeleteWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await DeleteAsync(ScytheSessionUrl, 2);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task DeleteUnknownPlaySessionReturnsNotFoundTest()
    {
        var response = await DeleteAsync(UnknownUrl, 1);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    private static string Iso(DateTime dateTime) => dateTime.ToString("O");
}
