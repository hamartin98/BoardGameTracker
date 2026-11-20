using BoardGameTracker.Database.Interceptors;

namespace BoardGameTracker.Api.Setup;

internal static class DatabaseInterceptorSetupExtensions
{
    public static IServiceCollection SetupDatabaseInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<UpdateInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();
        return services;
    }
}
