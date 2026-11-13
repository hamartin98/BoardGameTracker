using System.Reflection;
using System.Text.Json.Serialization;
using BoardGameTracker.Database;
using BoardGameTracker.Database.Interceptors;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSingleton<SoftDeleteInterceptor>();

builder.Services.AddDbContext<BoardGameContext>(
    (serviceProvider, options) =>
        options
            .UseInMemoryDatabase("BoardGameList")
            .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>())
);

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

var app = builder.Build();

using (var scope = app.Services.CreateAsyncScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<BoardGameContext>();
        await DbDataSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
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
