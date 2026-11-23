using System.Net;
using System.Net.Http.Headers;
using BoardGameTracker.Models.Imports.BoardRecord;
using BoardGameTracker.TestData;
using BoardGameTracker.TestData.Models;
using BoardGameTracker.ViewModels;

namespace BoardGameTracker.IntegrationTests.ControllerTests;

public class ImportsControllerTests : IntegrationTestBase
{
    private Task<HttpResponseMessage> ImportAsync(Stream file, string query = "")
    {
        StreamContent fileContent = new(file);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        MultipartFormDataContent content = new() { { fileContent, "file", "backup.json" } };
        return Client.PostAsync($"/imports/board-record{query}", content);
    }

    private Task<HttpResponseMessage> ImportAsync(BoardRecordBackup backup, string query = "")
    {
        return ImportAsync(ModelsTestData.ToStream(backup), query);
    }

    private async Task<int> CountAsync(string url)
    {
        var response = await Client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (
            await ReadAsync<PaginatedResponseVM<System.Text.Json.JsonElement>>(response)
        ).TotalCount;
    }

    // POST /imports/board-record

    [Fact]
    public async Task ImportMatchesSeededEntitiesTest()
    {
        var response = await ImportAsync(ModelsTestData.BoardRecordBackup);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ImportResultVM result = await ReadAsync<ImportResultVM>(response);
        Assert.False(result.DryRun);
        Assert.Equal(1, result.Players.Matched);
        Assert.Equal(1, result.Players.Created);
        Assert.Equal(1, result.BoardGames.Matched);
        Assert.Equal(2, result.BoardGames.Created);
        Assert.Equal(4, result.PlaySessions.Created);
        Assert.Equal(5, await CountAsync("/play-sessions"));
    }

    [Fact]
    public async Task ImportAgainAddsOnlyNewPlaysTest()
    {
        await ImportAsync(ModelsTestData.BoardRecordBackup);

        var response = await ImportAsync(ModelsTestData.BoardRecordBackupWithNewPlay);

        ImportResultVM result = await ReadAsync<ImportResultVM>(response);
        Assert.Equal(1, result.PlaySessions.Created);
        Assert.Equal(4, result.PlaySessions.AlreadyImported);
        Assert.Equal(6, await CountAsync("/play-sessions"));
    }

    [Fact]
    public async Task DryRunSavesNothingTest()
    {
        var response = await ImportAsync(ModelsTestData.BoardRecordBackup, "?dryRun=true");

        ImportResultVM result = await ReadAsync<ImportResultVM>(response);
        Assert.True(result.DryRun);
        Assert.Equal(4, result.PlaySessions.Created);
        Assert.Equal(1, await CountAsync("/play-sessions"));
    }

    [Fact]
    public async Task ImportInvalidFileReturnsBadRequestTest()
    {
        var response = await ImportAsync(new MemoryStream("not json"u8.ToArray()));

        await AssertProblemAsync(response, 400, "InvalidImportFileException");
    }
}
