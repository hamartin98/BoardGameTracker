using System.Net;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class PlayerScoresControllerTests : IntegrationTestBase
{
    private static readonly string AliceScoreUrl = $"/PlayerScores/{CommonTestData.Guid1}";
    private static readonly string UnknownUrl = $"/PlayerScores/{Guid.Empty}";

    // GET /PlayerScores

    [Fact]
    public async Task GetAllReturnsSeededPlayerScoresTest()
    {
        var response = await Client.GetAsync("/PlayerScores");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ReadAsync<PlayerScoresResponseVM>(response);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1, page.Page);
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.AliceScytheScoreVM,
            page.Items[0]
        );
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.BobScytheScoreVM,
            page.Items[1]
        );
    }

    public static TheoryData<string, Guid[]> FilterCases =>
        new()
        {
            { $"playerId={CommonTestData.Guid2}", [CommonTestData.Guid2] },
            { $"playerId={CommonTestData.DeletedGuid}", [] },
            { $"sessionId={CommonTestData.Guid1}", [CommonTestData.Guid1, CommonTestData.Guid2] },
            { $"sessionId={CommonTestData.Guid2}", [] },
            { "win=true", [CommonTestData.Guid1] },
            { "win=false", [CommonTestData.Guid2] },
            { $"playerId={CommonTestData.Guid1}&win=false", [] },
        };

    [Theory]
    [MemberData(nameof(FilterCases))]
    public async Task GetAllFiltersTest(string query, Guid[] expectedIds)
    {
        var response = await Client.GetAsync($"/PlayerScores?{query}");

        var page = await ReadAsync<PlayerScoresResponseVM>(response);
        Assert.Equal(expectedIds, page.Items.Select(e => e.MetaData.Id));
    }

    [Fact]
    public async Task GetAllPagesTest()
    {
        var response = await Client.GetAsync("/PlayerScores?page=2&pageSize=1");

        var page = await ReadAsync<PlayerScoresResponseVM>(response);
        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.BobScytheScoreVM,
            Assert.Single(page.Items)
        );
    }

    [Fact]
    public async Task GetAllOrdersTest()
    {
        var response = await Client.GetAsync("/PlayerScores?orderBy=score&orderDirection=ASC");

        var page = await ReadAsync<PlayerScoresResponseVM>(response);
        Assert.Equal(
            [CommonTestData.Guid2, CommonTestData.Guid1],
            page.Items.Select(e => e.MetaData.Id)
        );
    }

    [Fact]
    public async Task GetAllWithUnknownOrderByReturnsBadRequestTest()
    {
        var response = await Client.GetAsync("/PlayerScores?orderBy=nope");

        await AssertProblemAsync(response, 400, "InvalidOrderByException");
    }

    // GET /PlayerScores/{id}

    [Fact]
    public async Task GetByIdReturnsPlayerScoreTest()
    {
        var response = await Client.GetAsync(AliceScoreUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.AliceScytheScoreVM,
            await ReadAsync<PlayerScoreResponseVM>(response)
        );
    }

    [Fact]
    public async Task GetByIdOfUnknownPlayerScoreReturnsNotFoundTest()
    {
        var response = await Client.GetAsync(UnknownUrl);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // POST /PlayerScores

    [Fact]
    public async Task CreateTest()
    {
        var session = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync(
                "/PlaySessions",
                ViewModelsTestData.ScytheSessionWithoutExpansionsCreateVM
            )
        );
        var createVM = ViewModelsTestData.AliceScytheScoreCreateVM;
        createVM.SessionId = session.MetaData.Id;

        var response = await PostAsync("/PlayerScores", createVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<PlayerScoreResponseVM>(response);
        ViewModelsTestData.CheckPlayerScoreVM(createVM, created);
        Assert.Equal(1, created.MetaData.Version);

        Assert.NotNull(response.Headers.Location);
        var fetched = await ReadAsync<PlayerScoreResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckPlayerScoreResponseVM(created, fetched);

        var sessionScores = await ReadAsync<List<PlayerScoreResponseVM>>(
            await Client.GetAsync($"/PlaySessions/{session.MetaData.Id}/PlayerScores")
        );
        ViewModelsTestData.CheckPlayerScoreResponseVM(created, Assert.Single(sessionScores));
    }

    [Fact]
    public async Task CreateAfterDeletingPlayersScoreTest()
    {
        await Client.DeleteAsync($"{AliceScoreUrl}?version=1");

        var response = await PostAsync(
            "/PlayerScores",
            ViewModelsTestData.AliceScytheScoreCreateVM
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        ViewModelsTestData.CheckPlayerScoreVM(
            ViewModelsTestData.AliceScytheScoreCreateVM,
            await ReadAsync<PlayerScoreResponseVM>(response)
        );
    }

    [Fact]
    public async Task CreateSecondScoreOfPlayerInSessionReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/PlayerScores",
            ViewModelsTestData.AliceScytheScoreCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateForUnknownPlayerReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/PlayerScores",
            ViewModelsTestData.UnknownPlayerScytheScoreCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task CreateForPlayerNotInSessionReturnsUnprocessableTest()
    {
        var session = await ReadAsync<PlaySessionResponseVM>(
            await PostAsync("/PlaySessions", ViewModelsTestData.ScytheSessionWithoutPlayersCreateVM)
        );
        var createVM = ViewModelsTestData.AliceScytheScoreCreateVM;
        createVM.SessionId = session.MetaData.Id;

        var response = await PostAsync("/PlayerScores", createVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateForUnknownSessionReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/PlayerScores",
            ViewModelsTestData.AliceUnknownSessionScoreCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // PUT /PlayerScores/{id}

    [Fact]
    public async Task UpdateTest()
    {
        var response = await PutAsync(
            $"{AliceScoreUrl}?version=1",
            ViewModelsTestData.AliceScytheScoreUpdateVM
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<PlayerScoreResponseVM>(response);
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.AliceScytheScoreUpdatedVM,
            updated
        );

        var fetched = await ReadAsync<PlayerScoreResponseVM>(await Client.GetAsync(AliceScoreUrl));
        ViewModelsTestData.CheckPlayerScoreResponseVM(
            ViewModelsTestData.AliceScytheScoreUpdatedVM,
            fetched
        );
    }

    [Fact]
    public async Task UpdateWithStaleVersionReturnsConflictTest()
    {
        var response = await PutAsync(
            $"{AliceScoreUrl}?version=2",
            ViewModelsTestData.AliceScytheScoreUpdateVM
        );

        await AssertProblemAsync(response, 409, "VersionMismatchException");
    }

    [Fact]
    public async Task UpdateUnknownPlayerScoreReturnsNotFoundTest()
    {
        var response = await PutAsync(
            $"{UnknownUrl}?version=1",
            ViewModelsTestData.AliceScytheScoreUpdateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // DELETE /PlayerScores/{id}

    [Fact]
    public async Task DeleteTest()
    {
        var response = await Client.DeleteAsync($"{AliceScoreUrl}?version=1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(AliceScoreUrl)).StatusCode);

        var session = await ReadAsync<PlaySessionResponseVM>(
            await Client.GetAsync($"/PlaySessions/{CommonTestData.Guid1}")
        );
        Assert.Equal([CommonTestData.Guid2], session.PlayerScoreIds);
        Assert.Equal(1, session.MetaData.Version);
    }

    [Fact]
    public async Task DeleteWithStaleVersionReturnsConflictTest()
    {
        var response = await Client.DeleteAsync($"{AliceScoreUrl}?version=2");

        await AssertProblemAsync(response, 409, "VersionMismatchException");
    }

    [Fact]
    public async Task DeleteUnknownPlayerScoreReturnsNotFoundTest()
    {
        var response = await Client.DeleteAsync($"{UnknownUrl}?version=1");

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }
}
