using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BoardGameTracker.Api.Setup;

internal static class ControllerSetupExtensions
{
    public static IServiceCollection SetupControllers(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services
            .AddControllers(options =>
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(new KebabCaseParameterTransformer())
                )
            )
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );
        return services;
    }
}
