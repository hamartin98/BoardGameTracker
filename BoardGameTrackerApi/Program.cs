using System.Reflection;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Mappers;
using BoardGameTracker.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BoardGameContext>(options =>
    options
        .UseInMemoryDatabase("BoardGameList")
        .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
);

builder.Services.AddTransient<IBoardGameService, BoardGameService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IPlayerScoreService, PlayerScoreService>();
builder.Services.AddTransient<IPlaySessionService, PlaySessionService>();

var typeAdapterConfig = new TypeAdapterConfig();
builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();
typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IBoardGameTrackerMapper, BoardGameTrackerMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "Board game tracker API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
