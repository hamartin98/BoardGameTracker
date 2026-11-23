using System.Net;
using System.Net.Http.Json;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.ViewModels;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class PlayersControllerTests : IntegrationTestBase
{
    private static readonly string AliceUrl = $"/players/{CommonTestData.Guid1}";
    private static readonly string UnknownUrl = $"/players/{Guid.Empty}";

    // GET /players

    [Fact]
    public async Task GetAllReturnsSeededPlayersTest()
    {
        var response = await Client.GetAsync("/players");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ReadAsync<PlayersResponseVM>(response);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1, page.Page);
        ViewModelsTestData.CheckPlayerResponseVM(ViewModelsTestData.AliceVM, page.Items[0]);
        ViewModelsTestData.CheckPlayerResponseVM(ViewModelsTestData.BobVM, page.Items[1]);
    }

    [Theory]
    [InlineData("name=LIC", "Alice")]
    [InlineData("name=o", "Bob")]
    [InlineData("name=carol")]
    [InlineData("createdAtFrom=2000-01-01T00:00:00Z", "Alice", "Bob")]
    [InlineData("createdAtTo=2000-01-01T00:00:00Z")]
    public async Task GetAllFiltersTest(string query, params string[] expectedNames)
    {
        var response = await Client.GetAsync($"/players?{query}");

        var page = await ReadAsync<PlayersResponseVM>(response);
        Assert.Equal(expectedNames, page.Items.Select(e => e.Name));
    }

    [Fact]
    public async Task GetAllPagesTest()
    {
        var response = await Client.GetAsync("/players?page=2&pageSize=1");

        var page = await ReadAsync<PlayersResponseVM>(response);
        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        ViewModelsTestData.CheckPlayerResponseVM(
            ViewModelsTestData.BobVM,
            Assert.Single(page.Items)
        );
    }

    [Fact]
    public async Task GetAllOrdersTest()
    {
        var response = await Client.GetAsync("/players?orderBy=name&orderDirection=DESC");

        var page = await ReadAsync<PlayersResponseVM>(response);
        Assert.Equal(
            [ViewModelsTestData.BobVM.Name, ViewModelsTestData.AliceVM.Name],
            page.Items.Select(e => e.Name)
        );
    }

    [Fact]
    public async Task GetAllWithUnknownOrderByReturnsBadRequestTest()
    {
        var response = await Client.GetAsync("/players?orderBy=nope");

        await AssertProblemAsync(response, 400, "InvalidOrderByException");
    }

    // GET /players/{id}

    [Fact]
    public async Task GetByIdReturnsPlayerTest()
    {
        var response = await Client.GetAsync(AliceUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckPlayerResponseVM(
            ViewModelsTestData.AliceVM,
            await ReadAsync<PlayerResponseVM>(response)
        );
        Assert.Equal("\"1\"", response.Headers.ETag?.Tag);
        Assert.False(response.Headers.ETag?.IsWeak);
    }

    [Fact]
    public async Task GetByNonGuidIdReturnsNotFoundTest()
    {
        var response = await Client.GetAsync("/players/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateNonGuidIdReturnsNotFoundTest()
    {
        var response = await PutAsync("/players/abc", 1, ViewModelsTestData.AliceRenamedUpdateVM);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PascalCaseUrlStillResolvesTest()
    {
        var response = await Client.GetAsync($"/Players/{CommonTestData.Guid1}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdOfUnknownPlayerReturnsNotFoundTest()
    {
        var response = await Client.GetAsync(UnknownUrl);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    // POST /players

    [Fact]
    public async Task CreateTest()
    {
        var response = await PostAsync("/players", ViewModelsTestData.CarolCreateVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await ReadAsync<PlayerResponseVM>(response);
        ViewModelsTestData.CheckPlayerVM(ViewModelsTestData.CarolCreateVM, created);
        Assert.Equal(1, created.MetaData.Version);

        Assert.Equal("\"1\"", response.Headers.ETag?.Tag);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal($"/players/{created.MetaData.Id}", response.Headers.Location.AbsolutePath);
        var fetched = await ReadAsync<PlayerResponseVM>(
            await Client.GetAsync(response.Headers.Location)
        );
        ViewModelsTestData.CheckPlayerResponseVM(created, fetched);
    }

    [Fact]
    public async Task CreateWithEmptyNameReturnsBadRequestTest()
    {
        var response = await PostAsync("/players", ViewModelsTestData.EmptyNameCreateVM);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateWithDuplicateNameReturnsUnprocessableTest()
    {
        var response = await PostAsync("/players", ViewModelsTestData.AliceUppercaseCreateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    [Fact]
    public async Task CreateWithNameOfDeletedPlayerTest()
    {
        await DeleteAsync(AliceUrl, 1);

        var response = await PostAsync("/players", ViewModelsTestData.AliceUppercaseCreateVM);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // PUT /players/{id}

    [Fact]
    public async Task UpdateTest()
    {
        var response = await PutAsync(AliceUrl, 1, ViewModelsTestData.AliceRenamedUpdateVM);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadAsync<PlayerResponseVM>(response);
        ViewModelsTestData.CheckPlayerVM(ViewModelsTestData.AliceRenamedUpdateVM, updated);
        Assert.Equal(CommonTestData.Guid1, updated.MetaData.Id);
        Assert.Equal(2, updated.MetaData.Version);
        Assert.Equal("\"2\"", response.Headers.ETag?.Tag);

        var fetched = await ReadAsync<PlayerResponseVM>(await Client.GetAsync(AliceUrl));
        ViewModelsTestData.CheckPlayerResponseVM(updated, fetched);
    }

    [Fact]
    public async Task UpdateToOwnNameInOtherCaseTest()
    {
        var response = await PutAsync(AliceUrl, 1, ViewModelsTestData.AliceUppercaseUpdateVM);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ViewModelsTestData.CheckPlayerVM(
            ViewModelsTestData.AliceUppercaseUpdateVM,
            await ReadAsync<PlayerResponseVM>(response)
        );
    }

    [Fact]
    public async Task UpdateWithStaleVersionReturnsPreconditionFailedTest()
    {
        await PutAsync(AliceUrl, 1, ViewModelsTestData.AliceRenamedUpdateVM);

        var response = await PutAsync(AliceUrl, 1, ViewModelsTestData.AliceRenamedUpdateVM);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task UpdateWithoutIfMatchReturnsPreconditionRequiredTest()
    {
        var response = await PutAsync(AliceUrl, ViewModelsTestData.AliceRenamedUpdateVM);

        await AssertProblemAsync(response, 428, "PreconditionRequiredException");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("W/\"1\"")]
    [InlineData("*")]
    [InlineData("\"one\"")]
    public async Task UpdateWithInvalidIfMatchReturnsBadRequestTest(string ifMatch)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, AliceUrl)
        {
            Content = JsonContent.Create(ViewModelsTestData.AliceRenamedUpdateVM),
        };
        request.Headers.TryAddWithoutValidation("If-Match", ifMatch);

        var response = await Client.SendAsync(request);

        await AssertProblemAsync(response, 400, "InvalidIfMatchException");
    }

    [Fact]
    public async Task UpdateUnknownPlayerReturnsNotFoundTest()
    {
        var response = await PutAsync(UnknownUrl, 1, ViewModelsTestData.AliceRenamedUpdateVM);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }

    [Fact]
    public async Task UpdateToTakenNameReturnsUnprocessableTest()
    {
        var response = await PutAsync(AliceUrl, 1, ViewModelsTestData.AliceRenamedToBobUpdateVM);

        await AssertProblemAsync(response, 422, "BusinessRuleException");
    }

    // DELETE /players/{id}

    [Fact]
    public async Task DeleteCascadesToScoresTest()
    {
        var response = await DeleteAsync(AliceUrl, 1);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(AliceUrl)).StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Client.GetAsync($"/player-scores/{CommonTestData.Guid1}")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.OK,
            (await Client.GetAsync($"/player-scores/{CommonTestData.Guid2}")).StatusCode
        );
    }

    [Fact]
    public async Task DeleteWithStaleVersionReturnsPreconditionFailedTest()
    {
        var response = await DeleteAsync(AliceUrl, 2);

        await AssertProblemAsync(response, 412, "VersionMismatchException");
    }

    [Fact]
    public async Task DeleteWithoutIfMatchReturnsPreconditionRequiredTest()
    {
        var response = await Client.DeleteAsync(AliceUrl);

        await AssertProblemAsync(response, 428, "PreconditionRequiredException");
    }

    [Fact]
    public async Task DeleteUnknownPlayerReturnsNotFoundTest()
    {
        var response = await DeleteAsync(UnknownUrl, 1);

        await AssertProblemAsync(response, 404, "EntityNotFoundException");
    }
}
