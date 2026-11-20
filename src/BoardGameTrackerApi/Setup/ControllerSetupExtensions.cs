using System.Text.Json.Serialization;

namespace BoardGameTracker.Api.Setup;

internal static class ControllerSetupExtensions
{
    public static IServiceCollection SetupControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );
        return services;
    }
}
