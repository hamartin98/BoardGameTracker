using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Models.Imports;
using BoardGameTracker.Models.Recommendations;
using BoardGameTracker.Services;
using BoardGameTracker.Services.Imports;

namespace BoardGameTracker.Api.Setup;

internal static class ServiceSetupExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(Random.Shared);
        services.AddTransient<IBoardGameService, BoardGameService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IPlayerScoreService, PlayerScoreService>();
        services.AddTransient<IPlaySessionService, PlaySessionService>();
        services.AddTransient<IScoringCategoryService, ScoringCategoryService>();
        services.AddTransient<IRecommendationService, RecommendationService>();
        services.AddTransient<IImportService, BoardRecordImportService>();
        return services;
    }

    /// <summary>
    /// Binds the RecommendationConfig section; missing values keep the RecommendationOptions defaults
    /// </summary>
    public static IServiceCollection SetupRecommendationOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        RecommendationOptions options = new();
        configuration.GetSection("RecommendationConfig").Bind(options);
        services.AddSingleton(options);
        return services;
    }

    /// <summary>
    /// Binds the ImportConfig section; missing values keep the ImportOptions defaults
    /// </summary>
    public static IServiceCollection SetupImportOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ImportOptions options = new();
        configuration.GetSection("ImportConfig").Bind(options);
        services.AddSingleton(options);
        return services;
    }
}
