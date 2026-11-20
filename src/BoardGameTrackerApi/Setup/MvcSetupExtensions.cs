using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Api.Setup;

internal static class MvcSetupExtensions
{
    public static MvcOptions SetupFilters(this MvcOptions options)
    {
        List<int> statusCodes = [400, 404, 409, 422, 500];
        statusCodes.ForEach(code =>
            options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), code))
        );
        return options;
    }
}
