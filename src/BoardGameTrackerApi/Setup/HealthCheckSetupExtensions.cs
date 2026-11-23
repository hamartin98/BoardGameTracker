using BoardGameTracker.Database;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BoardGameTracker.Api.Setup;

internal static class HealthCheckSetupExtensions
{
    private const string ReadyTag = "ready";

    public static IServiceCollection SetupHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddDbContextCheck<BoardGameContext>(tags: [ReadyTag]);
        return services;
    }

    /// <summary>
    /// /health/live runs no checks; /health/ready also checks the database connection
    /// </summary>
    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks(
            "/health/ready",
            new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) }
        );
        return app;
    }
}
