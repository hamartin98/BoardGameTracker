using System.Reflection;
using Mapster;
using MapsterMapper;

namespace BoardGameTracker.Api.Setup;

internal static class MapperSetupExtensions
{
    public static IServiceCollection SetupMapper(this IServiceCollection services)
    {
        var typeAdapterConfig = new TypeAdapterConfig();
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        return services;
    }
}
