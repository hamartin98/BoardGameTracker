using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigurePlayer(this ModelBuilder modelBuilder)
    {
        AbstractEntityConfiguration.Configure<Player>(modelBuilder);
        modelBuilder.Entity<Player>(entity =>
        {
            entity.Ignore(e => e.Statistics);

            // Filtered so a soft-deleted player's name can be reused
            entity.HasIndex(e => e.NormalizedName).IsUnique().HasFilter("is_deleted = false");
        });
    }
}
