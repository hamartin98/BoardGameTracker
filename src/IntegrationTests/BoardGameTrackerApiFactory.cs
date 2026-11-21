using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BoardGameTracker.IntegrationTests;

/// <summary>
/// Runs the API in memory with its own seeded in-memory database
/// </summary>
public class BoardGameTrackerApiFactory : WebApplicationFactory<Program>
{
    private readonly string DatabaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("DatabaseType", "IN_MEMORY");
        builder.UseSetting("InMemoryDatabaseName", DatabaseName);
    }
}
