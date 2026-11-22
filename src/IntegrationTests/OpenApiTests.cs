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
    [InlineData("/BoardGames")]
    [InlineData("/BoardGames/{id}/ScoringCategories/Order")]
    [InlineData("/Players/{id}")]
    [InlineData("/PlayerScores")]
    [InlineData("/PlaySessions/Start")]
    [InlineData("/PlaySessions/{id}/ScoringCategories")]
    [InlineData("/Recommendations")]
    [InlineData("/ScoringCategories/{id}")]
    public async Task DocumentListsPathTest(string path)
    {
        using JsonDocument document = JsonDocument.Parse(await Client.GetStringAsync(DocumentUrl));

        Assert.True(document.RootElement.GetProperty("paths").TryGetProperty(path, out _));
    }
}
