using NLog;
using NLog.Web;

namespace BoardGameTracker.Api.Setup;

internal static class LoggerSetup
{
    public static NLog.ILogger SetupLogger()
    {
        var logConfig = new NLog.Config.LoggingConfiguration();
        var logToConsole = new NLog.Targets.ConsoleTarget("logconsole");
        logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logToConsole);
        LogManager.Configuration = logConfig;
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        return logger;
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        builder.Host.UseNLog();
        return builder;
    }
}
