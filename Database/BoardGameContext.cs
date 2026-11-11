using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database;

public class BoardGameContext : DbContext
{
    public BoardGameContext(DbContextOptions<BoardGameContext> options)
        : base(options) { }

    public DbSet<BoardGame> BoardGames { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<PlayerScore> PlayerScores { get; set; }

    public DbSet<PlaySession> PlaySessions { get; set; }
}
