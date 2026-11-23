using System.Net;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class BoardGamesControllerTests : IntegrationTestBase
{
    private static readonly string ScytheUrl = $"/board-games/{CommonTestData.Guid1}";
    private static readonly string FenrisUrl = $"/board-games/{CommonTestData.Guid2}";
    private static readonly string DeletedUrl = $"/board-games/{CommonTestData.DeletedGuid}";
    private static readonly string UnknownUrl = $"/board-games/{Guid.Empty}";

    // GET /board-games

    [Fact]
    public async Task GetAllReturnsSeededBoardGamesTest()
    {
        var response = await Client.GetAsync("/board-games");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ReadAsync<BoardGamesResponseVM>(response);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1, page.Page);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheVM, page.Items[0]);
        ViewModelsTestData.CheckBoardGameResponseVM(
            ViewModelsTestData.ScytheFenrisVM,
            page.Items[1]
        );
    }

    [Theory]
    [InlineData("name=FENRIS", "Scythe: The Rise of Fenris")]
    [InlineData("type=EXPANSION", "Scythe: The Rise of Fenris")]
    [InlineData("type=BASE_GAME", "Scythe")]
    [InlineData(
        "minPlayTime=01:50:00&maxPlayTime=02:00:00",
        "Scythe",
        "Scythe: The Rise of Fenris"
    )]
    [InlineData("minPlayerCount=2")]
    [InlineData("categories=ECONOMIC", "Scythe", "Scythe: The Rise of Fenris")]
    [InlineData("categories=ABSTRACT")]
    [InlineData("createdAtFrom=2000-01-01T00:00:00Z", "Scythe", "Scythe: The Rise of Fenris")]
    [InlineData("createdAtTo=2000-01-01T00:00:00Z")]
    public async Task GetAllFiltersTest(string query, params string[] expectedNames)
    {
        var response = await Client.GetAsync($"/board-games?{query}");

        var page = await ReadAsync<BoardGamesResponseVM>(response);
        Assert.Equal(expectedNames, page.Items.Select(e => e.Name));
    }

    [Fact]
    public async Task GetAllPagesTest()
    {
        var response = await Client.GetAsync("/board-games?page=2&pageSize=1");

        var page = await ReadAsync<BoardGamesResponseVM>(response);
        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        ViewModelsTestData.CheckBoardGameResponseVM(
            ViewModelsTestData.ScytheFenrisVM,
            Assert.Single(page.Items)
        );
    }

    [Fact]
    public async Task GetAllOrdersTest()
    {
        var response = await Client.GetAsync("/board-games?orderBy=name&orderDirection=DESC");

        var page = await ReadAsync<BoardGamesResponseVM>(response);
        Assert.Equal(
            [ViewModelsTestData.ScytheFenrisVM.Name, ViewModelsTestData.ScytheVM.Name],
            page.Items.Select(e => e.Name)
        );
    }

    [Fact]
    public async Task GetAllWithUnknownOrderByReturnsBadRequestTest()
    {
        var response = await Client.GetAsync("/board-games?orderBy=nope");

        await AssertProblemAsync(response, 400, "InvalidOrderByException");
    }

    // GET /board-games/{id}

    [Fact]
    public async Task GetByIdReturnsBoardGameTest()
    {
        var response = await Client.GetAsync(ScytheUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var boardGame = await ReadAsync<BoardGameResponseVM>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheVM, boardGame);
        Assert.Equal(CommonTestData.Guid1, boardGame.MetaData.Id);
        Assert.Equal(1, boardGame.MetaData.Version);
    }

    [Fact]
    public async Task GetByIdOfExpansionIncludesParentTest()
    {
        var response = await Client.GetAsync(FenrisUrl);

        var boardGame = await ReadAsync<BoardGameResponseVM>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheFenrisVM, boardGame);
        Assert.NotNull(boardGame.ExpansionOf);
        ViewModelsTestData.CheckBoardGameResponseVM(
            ViewModelsTestData.ScytheVM,
            boardGame.ExpansionOf
        );
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("deleted")]
    public async Task GetByIdOfMissingBoardGameReturnsNotFoundTest(string which)
    {
        var response = await Client.GetAsync(which == "deleted" ? DeletedUrl : UnknownUrl);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // GET /board-games/{id}/expansions

    [Fact]
    public async Task GetExpansionsTest()
    {
        var response = await Client.GetAsync($"{ScytheUrl}/expansions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expansions = await ReadAsync<List<BoardGameResponseVM>>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(
            ViewModelsTestData.ScytheFenrisVM,
            Assert.Single(expansions)
        );
    }

    [Fact]
    public async Task GetExpansionsOfUnknownBoardGameReturnsNotFoundTest()
    {
        var response = await Client.GetAsync($"{UnknownUrl}/expansions");

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // POST /board-games

    [Fact]
    public async Task CreateTest()
    {
        var response = await PostAsync("/board-games", ViewModelsTestData.AzulCreateVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<BoardGameResponseVM>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.AzulVM, created);
        Assert.Equal(1, created.MetaData.Version);

        Assert.NotNull(response.Headers.Location);
        var fetched = await ReadAsync<BoardGameResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.AzulVM, fetched);
    }

    [Fact]
    public async Task CreateExpansionTest()
    {
        var response = await PostAsync("/board-games", ViewModelsTestData.ScytheInvadersCreateVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        ViewModelsTestData.CheckBoardGameResponseVM(
            ViewModelsTestData.ScytheInvadersVM,
            await ReadAsync<BoardGameResponseVM>(response)
        );
        var expansions = await ReadAsync<List<BoardGameResponseVM>>(
            await Client.GetAsync($"{ScytheUrl}/expansions")
        );
        Assert.Equal(2, expansions.Count);
    }

    [Fact]
    public async Task CreateWithInvalidBodyReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/board-games",
            ViewModelsTestData.AzulInvalidPlayTimeCreateVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWithDuplicateNameReturnsUnprocessableTest()
    {
        var response = await PostAsync("/board-games", ViewModelsTestData.ScytheUppercaseCreateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateExpansionOfExpansionReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/board-games",
            ViewModelsTestData.ExpansionOfFenrisCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateBaseGameWithParentReturnsUnprocessableTest()
    {
        var response = await PostAsync("/board-games", ViewModelsTestData.AzulWithParentCreateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateExpansionOfDeletedGameReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/board-games",
            ViewModelsTestData.ExpansionOfDeletedGameCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // PUT /board-games/{id}

    [Fact]
    public async Task UpdateTest()
    {
        var response = await PutAsync(ScytheUrl, 1, ViewModelsTestData.ScytheUpdateVM);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<BoardGameResponseVM>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheUpdatedVM, updated);
        Assert.Equal(2, updated.MetaData.Version);

        var fetched = await ReadAsync<BoardGameResponseVM>(await Client.GetAsync(ScytheUrl));
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheUpdatedVM, fetched);
        Assert.Equal(2, fetched.MetaData.Version);
    }

    [Fact]
    public async Task UpdateExpansionTest()
    {
        var response = await PutAsync(FenrisUrl, 1, ViewModelsTestData.ScytheFenrisUpdateVM);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<BoardGameResponseVM>(response);
        ViewModelsTestData.CheckBoardGameResponseVM(ViewModelsTestData.ScytheFenrisVM, updated);
    }

    [Fact]
    public async Task UpdateWithStaleVersionReturnsPreconditionFailedTest()
    {
        await PutAsync(ScytheUrl, 1, ViewModelsTestData.ScytheUpdateVM);

        var response = await PutAsync(ScytheUrl, 1, ViewModelsTestData.ScytheUpdateVM);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task UpdateUnknownBoardGameReturnsNotFoundTest()
    {
        var response = await PutAsync(UnknownUrl, 1, ViewModelsTestData.ScytheUpdateVM);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task UpdateToTakenNameReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            ScytheUrl,
            1,
            ViewModelsTestData.ScytheRenamedToFenrisUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateBaseGameWithParentReturnsUnprocessableTest()
    {
        var response = await PutAsync(ScytheUrl, 1, ViewModelsTestData.ScytheWithParentUpdateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateExpansionWithoutParentReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            FenrisUrl,
            1,
            ViewModelsTestData.ScytheFenrisWithoutParentUpdateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    // GET /board-games/{id}/scoring-categories

    [Fact]
    public async Task GetScoringCategoriesTest()
    {
        var response = await Client.GetAsync($"{ScytheUrl}/scoring-categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckScoringCategoryResponseVMs(
            [ViewModelsTestData.ScytheStarsVM, ViewModelsTestData.ScytheTerritoriesVM],
            await ReadAsync<List<ScoringCategoryResponseVM>>(response)
        );
    }

    [Fact]
    public async Task GetScoringCategoriesOfUnknownBoardGameReturnsNotFoundTest()
    {
        var response = await Client.GetAsync($"{UnknownUrl}/scoring-categories");

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // PUT /board-games/{id}/scoring-categories/order

    [Fact]
    public async Task ReorderScoringCategoriesTest()
    {
        var expected = ViewModelsTestData.ScytheTerritoriesVM;
        expected.Order = 0;
        expected.MetaData.Version = 2;
        var expectedStars = ViewModelsTestData.ScytheStarsVM;
        expectedStars.Order = 1;
        expectedStars.MetaData.Version = 2;

        var response = await PutAsync(
            $"{ScytheUrl}/scoring-categories/order",
            new List<Guid> { CommonTestData.Guid2, CommonTestData.Guid1 }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckScoringCategoryResponseVMs(
            [expected, expectedStars],
            await ReadAsync<List<ScoringCategoryResponseVM>>(response)
        );
        ViewModelsTestData.CheckScoringCategoryResponseVMs(
            [expected, expectedStars],
            await ReadAsync<List<ScoringCategoryResponseVM>>(
                await Client.GetAsync($"{ScytheUrl}/scoring-categories")
            )
        );
    }

    [Fact]
    public async Task ReorderScoringCategoriesWithIncompleteListReturnsUnprocessableTest()
    {
        var response = await PutAsync(
            $"{ScytheUrl}/scoring-categories/order",
            new List<Guid> { CommonTestData.Guid1 }
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task ReorderScoringCategoriesOfUnknownBoardGameReturnsNotFoundTest()
    {
        var response = await PutAsync($"{UnknownUrl}/scoring-categories/order", new List<Guid>());

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // DELETE /board-games/{id}

    [Fact]
    public async Task DeleteCascadesToExpansionsAndSessionsTest()
    {
        var response = await DeleteAsync(ScytheUrl, 1);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(ScytheUrl)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(FenrisUrl)).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Client.GetAsync($"/play-sessions/{CommonTestData.Guid1}")).StatusCode
        );
        var categories = await ReadAsync<ScoringCategoriesResponseVM>(
            await Client.GetAsync("/scoring-categories")
        );
        Assert.Empty(categories.Items);
    }

    [Fact]
    public async Task DeleteWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await DeleteAsync(ScytheUrl, 2);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task DeleteUnknownBoardGameReturnsNotFoundTest()
    {
        var response = await DeleteAsync(UnknownUrl, 1);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }
}
