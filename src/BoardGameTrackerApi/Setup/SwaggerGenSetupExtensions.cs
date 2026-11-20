using System.Reflection;

namespace BoardGameTracker.Api.Setup;

internal static class SwaggerGenSetupExtensions
{
    public static IServiceCollection SetupSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(config =>
        {
            Assembly[] documentedAssemblies =
            [
                Assembly.GetExecutingAssembly(),
                typeof(BoardGameTracker.ViewModels.BoardGameResponseVM).Assembly,
            ];
            foreach (var assembly in documentedAssemblies)
            {
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });
        return services;
    }
}
