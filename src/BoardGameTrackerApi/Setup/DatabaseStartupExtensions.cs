using BoardGameTracker.Api.Configuration;
using BoardGameTracker.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BoardGameTracker.Api.Setup;

internal static class DatabaseStartupExtensions
{
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Waits for the Postgres server, then applies pending migrations, or fails when applying them is turned off
    /// </summary>
    public static async Task PrepareDatabaseAsync(this WebApplication app)
    {
        PostgresConfiguration config =
            app.Configuration.GetSection("PostgresConfig").Get<PostgresConfiguration>() ?? new();
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<BoardGameContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BoardGameContext>>();

        List<string> pending = await GetPendingMigrationsAsync(context, logger);
        if (pending.Count == 0)
        {
            logger.LogInformation("Database schema is up to date");
            return;
        }
        if (!config.ApplyMigrationsOnStartup)
        {
            throw new InvalidOperationException(
                $"The database has pending migrations: {string.Join(", ", pending)}. Apply them with "
                    + "'dotnet ef database update' or set PostgresConfig:ApplyMigrationsOnStartup to true."
            );
        }

        logger.LogInformation("Applying migrations: {migrations}", string.Join(", ", pending));
        await context.Database.MigrateAsync();
        logger.LogInformation("Migrations applied");
    }

    /// <summary>
    /// Retries while the server is unreachable, up to ConnectTimeout; a missing database counts as all migrations pending
    /// </summary>
    private static async Task<List<string>> GetPendingMigrationsAsync(
        BoardGameContext context,
        ILogger logger
    )
    {
        DateTime deadline = DateTime.UtcNow + ConnectTimeout;
        TimeSpan delay = TimeSpan.FromSeconds(1);
        while (true)
        {
            try
            {
                return [.. await context.Database.GetPendingMigrationsAsync()];
            }
            catch (NpgsqlException exception)
                when (exception.IsTransient && DateTime.UtcNow + delay < deadline)
            {
                logger.LogWarning(
                    "Database not reachable ({message}), retrying in {delay}s",
                    exception.Message,
                    delay.TotalSeconds
                );
                await Task.Delay(delay);
                delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, MaxRetryDelay.Ticks));
            }
        }
    }
}
