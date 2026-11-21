using BoardGameTracker.TestData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameTracker.IntegrationTests;

/// <summary>
/// Runs the API in memory with its own seeded in-memory database
/// </summary>
public class BoardGameTrackerApiFactory : WebApplicationFactory<Program>
{
    private readonly string DatabaseName = Guid.NewGuid().ToString();

    /// <summary>
    /// Clock used by the services
    /// </summary>
    public FixedTimeProvider Clock { get; } = new(CommonTestData.DateTime);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("DatabaseType", "IN_MEMORY");
        builder.UseSetting("InMemoryDatabaseName", DatabaseName);
        builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(Clock));
    }
}
