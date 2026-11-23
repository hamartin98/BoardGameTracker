using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigureExternalReference(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExternalReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.Property(e => e.EntityType).HasMaxLength(50);

            entity.HasIndex(e => new { e.Source, e.ExternalId }).IsUnique();
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });
    }
}
