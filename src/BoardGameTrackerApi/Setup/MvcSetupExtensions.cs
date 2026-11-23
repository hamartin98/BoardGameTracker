using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Api.Setup;

internal static class MvcSetupExtensions
{
    public static MvcOptions SetupProblemResponses(this MvcOptions options)
    {
        options.Conventions.Add(new ProblemResponsesConvention());
        return options;
    }
}
