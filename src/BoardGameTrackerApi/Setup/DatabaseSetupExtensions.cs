using BoardGameTracker.Api.Configuration;
using BoardGameTracker.Database;
using BoardGameTracker.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BoardGameTracker.Api.Setup;

internal static class DatabaseSetupExtensions
{
    public static WebApplicationBuilder ConfigureDatabase(
        this WebApplicationBuilder builder,
        DatabaseType databaseType,
        string connectionString
    )
    {
        // Configurable so integration tests can give each test its own database
        string inMemoryDatabaseName = builder.Configuration.GetValue(
            "InMemoryDatabaseName",
            "BoardGameList"
        )!;
        builder.Services.AddDbContext<BoardGameContext>(
            (serviceProvider, options) =>
            {
                if (databaseType == DatabaseType.IN_MEMORY)
                {
                    options.ConfigureInMemoryOptions(inMemoryDatabaseName);
                }
                else if (databaseType == DatabaseType.POSTGRES)
                {
                    options.ConfigurePostgresOptions(connectionString);
                }

                options
                    .AddInterceptors(
                        serviceProvider.GetRequiredService<UpdateInterceptor>(),
                        serviceProvider.GetRequiredService<SoftDeleteInterceptor>()
                    )
                    .UseSnakeCaseNamingConvention();
            }
        );

        return builder;
    }

    public static WebApplicationBuilder SetupDatabase(
        this WebApplicationBuilder builder,
        DatabaseType databaseType
    )
    {
        string connectionString = string.Empty;
        if (databaseType == DatabaseType.POSTGRES)
        {
            PostgresConfiguration pgConfig = new();
            builder.Configuration.GetSection("PostgresConfig").Bind(pgConfig);
            pgConfig.Print();
            connectionString = pgConfig.GetConnectionString();
        }
        builder.ConfigureDatabase(databaseType, connectionString);
        return builder;
    }

    private static DbContextOptionsBuilder ConfigureInMemoryOptions(
        this DbContextOptionsBuilder optionsBuilder,
        string databaseName
    )
    {
        optionsBuilder
            .UseInMemoryDatabase(databaseName)
            .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        return optionsBuilder;
    }

    private static DbContextOptionsBuilder ConfigurePostgresOptions(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString
    )
    {
        optionsBuilder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly("Migrations")
        );
        return optionsBuilder;
    }
}
