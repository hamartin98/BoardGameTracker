using System.Reflection;
using System.Text.Json.Serialization;
using BoardGameTracker.Api.Configuration;
using BoardGameTracker.Database;
using BoardGameTracker.Database.Interceptors;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NLog;
using NLog.Web;

var logConfig = new NLog.Config.LoggingConfiguration();
var logToConsole = new NLog.Targets.ConsoleTarget("logconsole");
logConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logToConsole);
LogManager.Configuration = logConfig;
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();

    builder
        .Services.AddControllers()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        );
    builder.Services.AddSwaggerGen(config =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        config.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    });

    DatabaseType databaseType = builder.Configuration.GetValue<DatabaseType>("DatabaseType");
    logger.Info($"Database type: {databaseType}");

    if (databaseType == DatabaseType.IN_MEMORY)
    {
        builder.Services.AddDbContext<BoardGameContext>(
            (serviceProvider, options) =>
                options
                    .UseInMemoryDatabase("BoardGameList")
                    .ConfigureWarnings(config =>
                        config.Ignore(InMemoryEventId.TransactionIgnoredWarning)
                    )
                    .AddInterceptors(
                        serviceProvider.GetRequiredService<SoftDeleteInterceptor>(),
                        serviceProvider.GetRequiredService<UpdateInterceptor>()
                    )
                    .UseSnakeCaseNamingConvention()
        );
    }
    else if (databaseType == DatabaseType.POSTGRES)
    {
        PostgresConfguration pgConfig = new();
        builder.Configuration.GetSection("PostgresConfig").Bind(pgConfig);
        pgConfig.Print();
        builder.Services.AddDbContext<BoardGameContext>(options =>
        {
            options
                .UseNpgsql(
                    pgConfig.GetConnectionString(),
                    options => options.MigrationsAssembly("Migrations")
                )
                .UseSnakeCaseNamingConvention();
        });
    }

    builder.Services.AddSingleton<SoftDeleteInterceptor>();
    builder.Services.AddSingleton<UpdateInterceptor>();

    builder.Services.AddTransient<IBoardGameService, BoardGameService>();
    builder.Services.AddTransient<IPlayerService, PlayerService>();
    builder.Services.AddTransient<IPlayerScoreService, PlayerScoreService>();
    builder.Services.AddTransient<IPlaySessionService, PlaySessionService>();

    var typeAdapterConfig = new TypeAdapterConfig();
    builder.Services.AddSingleton(typeAdapterConfig);
    builder.Services.AddScoped<IMapper, ServiceMapper>();
    typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

    builder.Services.AddExceptionHandler<ExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddMvc(o =>
    {
        o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 400));
        o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 404));
        o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 409));
        o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 422));
        o.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), 500));
    });

    var app = builder.Build();

    if (databaseType == DatabaseType.IN_MEMORY)
    {
        using var scope = app.Services.CreateAsyncScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<BoardGameContext>();
            await DbDataSeeder.Seed(context);
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "Board game tracker API");
        });
    }

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
