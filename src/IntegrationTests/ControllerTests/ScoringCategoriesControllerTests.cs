using System.Net;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class ScoringCategoriesControllerTests : IntegrationTestBase
{
    private static readonly string StarsUrl = $"/scoring-categories/{CommonTestData.Guid1}";
    private static readonly string TerritoriesUrl = $"/scoring-categories/{CommonTestData.Guid2}";
    private static readonly string UnknownUrl = $"/scoring-categories/{Guid.Empty}";

    // GET /scoring-categories

    [Fact]
    public async Task GetAllReturnsSeededScoringCategoriesTest()
    {
        var response = await Client.GetAsync("/scoring-categories?orderBy=id");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ReadAsync<ScoringCategoriesResponseVM>(response);
        Assert.Equal(3, page.TotalCount);
        ViewModelsTestData.CheckScoringCategoryResponseVMs(
            [
                ViewModelsTestData.ScytheStarsVM,
                ViewModelsTestData.ScytheTerritoriesVM,
                ViewModelsTestData.FenrisBonusVM,
            ],
            page.Items
        );
    }

    public static TheoryData<string, Guid[]> FilterCases =>
        new()
        {
            {
                $"boardGameId={CommonTestData.Guid1}&orderBy=id",
                [CommonTestData.Guid1, CommonTestData.Guid2]
            },
            { $"boardGameId={CommonTestData.Guid2}", [CommonTestData.Guid3] },
            { "name=TERR", [CommonTestData.Guid2] },
            { $"boardGameId={CommonTestData.Guid2}&name=star", [] },
            { "orderBy=order&orderDirection=DESC&pageSize=1", [CommonTestData.Guid2] },
        };

    [Theory]
    [MemberData(nameof(FilterCases))]
    public async Task GetAllFiltersTest(string query, Guid[] expectedIds)
    {
        var response = await Client.GetAsync($"/scoring-categories?{query}");

        var page = await ReadAsync<ScoringCategoriesResponseVM>(response);
        Assert.Equal(expectedIds, page.Items.Select(e => e.MetaData.Id));
    }

    [Fact]
    public async Task GetAllWithUnknownOrderByReturnsBadRequestTest()
    {
        var response = await Client.GetAsync("/scoring-categories?orderBy=nope");

        await AssertProblemAsync(response, 400, "InvalidOrderByException");
    }

    // GET /scoring-categories/{id}

    [Fact]
    public async Task GetByIdReturnsScoringCategoryTest()
    {
        var response = await Client.GetAsync(TerritoriesUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckScoringCategoryResponseVM(
            ViewModelsTestData.ScytheTerritoriesVM,
            await ReadAsync<ScoringCategoryResponseVM>(response)
        );
    }

    [Fact]
    public async Task GetByIdOfUnknownScoringCategoryReturnsNotFoundTest()
    {
        var response = await Client.GetAsync(UnknownUrl);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // POST /scoring-categories

    [Fact]
    public async Task CreateAppendsToScoringOrderTest()
    {
        var response = await PostAsync(
            "/scoring-categories",
            ViewModelsTestData.ScytheResourcesCreateVM
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<ScoringCategoryResponseVM>(response);
        ViewModelsTestData.CheckScoringCategoryVM(
            ViewModelsTestData.ScytheResourcesCreateVM,
            created
        );
        Assert.Equal(2, created.Order);
        Assert.Equal(1, created.MetaData.Version);

        Assert.NotNull(response.Headers.Location);
        var fetched = await ReadAsync<ScoringCategoryResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckScoringCategoryResponseVM(created, fetched);
    }

    [Fact]
    public async Task CreateWithDuplicateNameReturnsUnprocessableTest()
    {
        var response = await PostAsync(
            "/scoring-categories",
            ViewModelsTestData.ScytheStarsUppercaseCreateVM
        );

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateForDeletedBoardGameReturnsNotFoundTest()
    {
        var response = await PostAsync(
            "/scoring-categories",
            ViewModelsTestData.DeletedBoardGameCategoryCreateVM
        );

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task CreateWithEmptyNameReturnsBadRequestTest()
    {
        var response = await PostAsync(
            "/scoring-categories",
            ViewModelsTestData.EmptyNameCategoryCreateVM
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // PUT /scoring-categories/{id}

    [Fact]
    public async Task UpdateTest()
    {
        var response = await PutAsync(StarsUrl, 1, ViewModelsTestData.StarTokensUpdateVM);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckScoringCategoryResponseVM(
            ViewModelsTestData.ScytheStarTokensVM,
            await ReadAsync<ScoringCategoryResponseVM>(response)
        );
        ViewModelsTestData.CheckScoringCategoryResponseVM(
            ViewModelsTestData.ScytheStarTokensVM,
            await ReadAsync<ScoringCategoryResponseVM>(await Client.GetAsync(StarsUrl))
        );
    }

    [Fact]
    public async Task UpdateWithDuplicateNameReturnsUnprocessableTest()
    {
        var response = await PutAsync(StarsUrl, 1, ViewModelsTestData.TerritoriesUppercaseUpdateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task UpdateWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await PutAsync(StarsUrl, 2, ViewModelsTestData.StarTokensUpdateVM);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task UpdateUnknownScoringCategoryReturnsNotFoundTest()
    {
        var response = await PutAsync(UnknownUrl, 1, ViewModelsTestData.StarTokensUpdateVM);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // DELETE /scoring-categories/{id}

    [Fact]
    public async Task DeleteRenumbersLaterCategoriesTest()
    {
        var created = await ReadAsync<ScoringCategoryResponseVM>(
            await PostAsync("/scoring-categories", ViewModelsTestData.ScytheResourcesCreateVM)
        );
        await DeleteAsync($"/player-scores/{CommonTestData.Guid1}", 1);
        await DeleteAsync($"/player-scores/{CommonTestData.Guid2}", 1);

        var response = await DeleteAsync(TerritoriesUrl, 1);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(TerritoriesUrl)).StatusCode);
        var resources = await ReadAsync<ScoringCategoryResponseVM>(
            await Client.GetAsync($"/scoring-categories/{created.MetaData.Id}")
        );
        Assert.Equal(1, resources.Order);
        Assert.Equal(2, resources.MetaData.Version);
    }

    [Fact]
    public async Task DeleteCategoryWithScoresReturnsUnprocessableTest()
    {
        var response = await DeleteAsync(StarsUrl, 1);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task DeleteWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await DeleteAsync(StarsUrl, 2);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task DeleteUnknownScoringCategoryReturnsNotFoundTest()
    {
        var response = await DeleteAsync(UnknownUrl, 1);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }
}
