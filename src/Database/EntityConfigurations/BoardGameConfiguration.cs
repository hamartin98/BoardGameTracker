using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigureBoardGame(this ModelBuilder modelBuilder)
    {
        AbstractEntityConfiguration.Configure<BoardGame>(modelBuilder);
        modelBuilder.Entity<BoardGame>(entity =>
        {
            entity
                .HasMany(e => e.Expansions)
                .WithOne(e => e.ExpansionOf)
                .HasForeignKey(e => e.ExpansionOfId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.NormalizedName).IsUnique().HasFilter("is_deleted = false");
        });
    }
}
