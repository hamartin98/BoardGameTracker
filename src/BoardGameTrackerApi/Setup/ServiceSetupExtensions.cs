using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Services;

namespace BoardGameTracker.Api.Setup;

internal static class ServiceSetupExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IBoardGameService, BoardGameService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IPlayerScoreService, PlayerScoreService>();
        services.AddTransient<IPlaySessionService, PlaySessionService>();
        services.AddTransient<IScoringCategoryService, ScoringCategoryService>();
        return services;
    }
}
