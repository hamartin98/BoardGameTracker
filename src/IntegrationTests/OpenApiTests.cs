using System.Net;
using System.Text.Json;

namespace BoardGameTracker.IntegrationTests;

/// <summary>
/// Checks that the Swagger document is generated and lists the endpoints
/// </summary>
public class OpenApiTests : IntegrationTestBase
{
    private const string DocumentUrl = "/swagger/v1/swagger.json";

    [Fact]
    public async Task DocumentIsGeneratedTest()
    {
        var response = await Client.GetAsync(DocumentUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/board-games")]
    [InlineData("/board-games/{id}/scoring-categories/order")]
    [InlineData("/players/{id}")]
    [InlineData("/player-scores")]
    [InlineData("/play-sessions/{id}/finish")]
    [InlineData("/play-sessions/{id}/scoring-categories")]
    [InlineData("/recommendations")]
    [InlineData("/scoring-categories/{id}")]
    public async Task DocumentListsPathTest(string path)
    {
        using JsonDocument document = JsonDocument.Parse(await Client.GetStringAsync(DocumentUrl));

        Assert.True(document.RootElement.GetProperty("paths").TryGetProperty(path, out _));
    }

    [Theory]
    [InlineData("/players", "get", "200,400,500")]
    [InlineData("/players", "post", "201,400,404,422,500")]
    [InlineData("/players/{id}", "get", "200,400,404,500")]
    [InlineData("/players/{id}", "put", "200,400,404,412,422,428,500")]
    [InlineData("/players/{id}", "delete", "204,400,404,412,422,428,500")]
    [InlineData("/play-sessions/{id}/finish", "post", "200,400,404,412,422,428,500")]
    [InlineData("/recommendations", "get", "200,400,500")]
    public async Task DocumentListsStatusCodesTest(string path, string method, string statusCodes)
    {
        using JsonDocument document = JsonDocument.Parse(await Client.GetStringAsync(DocumentUrl));

        var responses = document
            .RootElement.GetProperty("paths")
            .GetProperty(path)
            .GetProperty(method)
            .GetProperty("responses")
            .EnumerateObject()
            .Select(e => e.Name)
            .Order();
        Assert.Equal(statusCodes.Split(',').Order(), responses);
    }
}
