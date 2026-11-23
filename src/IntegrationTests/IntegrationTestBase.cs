using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BoardGameTracker.TestData;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.IntegrationTests;

/// <summary>
/// Gives every test a fresh app and database, seeded like the in-memory development database
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly BoardGameTrackerApiFactory Factory = new();

    protected readonly HttpClient Client;

    protected FixedTimeProvider Clock => Factory.Clock;

    protected IntegrationTestBase()
    {
        Client = Factory.CreateClient();
    }

    protected Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        return Client.PostAsJsonAsync(url, body, JsonOptions);
    }

    protected Task<HttpResponseMessage> PutAsync<T>(string url, T body)
    {
        return Client.PutAsJsonAsync(url, body, JsonOptions);
    }

    /// <summary>
    /// PUT with the If-Match header set to the ETag of the given version
    /// </summary>
    protected Task<HttpResponseMessage> PutAsync<T>(string url, int version, T body)
    {
        return SendAsync(
            new(HttpMethod.Put, url) { Content = JsonContent.Create(body, options: JsonOptions) },
            version
        );
    }

    /// <summary>
    /// DELETE with the If-Match header set to the ETag of the given version
    /// </summary>
    protected Task<HttpResponseMessage> DeleteAsync(string url, int version)
    {
        return SendAsync(new(HttpMethod.Delete, url), version);
    }

    /// <summary>
    /// POST without a body, with the If-Match header set to the ETag of the given version
    /// </summary>
    protected Task<HttpResponseMessage> PostEmptyAsync(string url, int version)
    {
        return SendAsync(new(HttpMethod.Post, url), version);
    }

    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, int version)
    {
        request.Headers.IfMatch.Add(new($"\"{version}\""));
        return Client.SendAsync(request);
    }

    protected static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        T? result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(result);
        return result;
    }

    /// <summary>
    /// Asserts the status code and that the body is a ProblemDetails of the given exception type
    /// </summary>
    protected static async Task AssertProblemAsync(
        HttpResponseMessage response,
        int expectedStatus,
        string expectedType
    )
    {
        Assert.Equal(expectedStatus, (int)response.StatusCode);
        ProblemDetails problem = await ReadAsync<ProblemDetails>(response);
        Assert.Equal(expectedType, problem.Type);
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
