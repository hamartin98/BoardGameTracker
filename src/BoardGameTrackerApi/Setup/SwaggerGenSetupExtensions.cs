using System.Reflection;

namespace BoardGameTracker.Api.Setup;

internal static class SwaggerGenSetupExtensions
{
    public static IServiceCollection SetupSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(config =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            config.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });
        return services;
    }
}
