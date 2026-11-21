using BoardGameTracker.Api.Configuration;
using BoardGameTracker.Api.Middlewares;
using BoardGameTracker.Api.Setup;
using BoardGameTracker.Database;
using NLog;

var logger = LoggerSetup.SetupLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLogging();

    builder.Services.SetupControllers();

    builder.Services.SetupSwaggerGen();

    DatabaseType databaseType = builder.Configuration.GetValue<DatabaseType>("DatabaseType");
    logger.Info($"Database type: {databaseType}");
    builder.Services.SetupDatabaseInterceptors();
    builder.SetupDatabase(databaseType);

    builder.Services.SetupServices();

    builder.Services.SetupMapper();

    builder.Services.AddExceptionHandler<ExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddMvc(options => options.SetupFilters());

    var app = builder.Build();

    if (databaseType == DatabaseType.IN_MEMORY)
    {
        using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<BoardGameContext>();
        await DbDataSeeder.Seed(context);
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
// EF Core design-time tools (dotnet ef) abort the host on purpose after Build()
catch (Exception exception) when (exception is not HostAbortedException)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
