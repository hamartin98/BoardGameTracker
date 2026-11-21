using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigurePlayerScore(this ModelBuilder modelBuilder)
    {
        AbstractEntityConfiguration.Configure<PlayerScore>(modelBuilder);
        modelBuilder.Entity<PlayerScore>(entity =>
        {
            entity
                .HasOne(e => e.Player)
                .WithMany(e => e.PlayerScores)
                .HasForeignKey(e => e.PlayerId)
                .IsRequired();

            entity
                .HasOne(e => e.Session)
                .WithMany(e => e.PlayerScores)
                .HasForeignKey(e => e.SessionId)
                .IsRequired();

            // One score per player per session; filtered so a deleted score can be re-added
            entity
                .HasIndex(e => new { e.SessionId, e.PlayerId })
                .IsUnique()
                .HasFilter("is_deleted = false");
        });
    }
}
