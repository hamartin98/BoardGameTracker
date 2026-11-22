using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigurePlaySession(this ModelBuilder modelBuilder)
    {
        AbstractEntityConfiguration.Configure<PlaySession>(modelBuilder);
        modelBuilder.Entity<PlaySession>(entity =>
        {
            entity
                .HasOne(e => e.BoardGame)
                .WithMany()
                .HasForeignKey(e => e.BoardGameId)
                .IsRequired();

            entity
                .HasMany(e => e.UsedExpansions)
                .WithMany()
                .UsingEntity(join => join.ToTable("play_session_expansions"));

            entity.Ignore(e => e.UsedExpansionIds);

            entity
                .HasMany(e => e.Players)
                .WithMany()
                .UsingEntity(join => join.ToTable("play_session_players"));

            entity.Ignore(e => e.PlayerIds);
            entity.Ignore(e => e.RandomStartPlayer);

            entity
                .HasOne(e => e.StartPlayer)
                .WithMany()
                .HasForeignKey(e => e.StartPlayerId)
                .IsRequired(false);

            entity.HasIndex(e => e.StartTime);
        });
    }
}
