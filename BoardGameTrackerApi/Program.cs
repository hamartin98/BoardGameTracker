using System.Reflection;
using BoardGameTracker.Database;
using BoardGameTracker.Interfaces.Services;
using BoardGameTracker.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BoardGameContext>(options =>
    options.UseInMemoryDatabase("BoardGameList")
);

builder.Services.AddTransient<IBoardGameService, BoardGameService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IPlayerScoreService, PlayerScoreService>();
builder.Services.AddTransient<IPlaySessionService, PlaySessionService>();

builder.Services.AddMapster();


TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
var typeAdapterConfig = new TypeAdapterConfig();
builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

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
