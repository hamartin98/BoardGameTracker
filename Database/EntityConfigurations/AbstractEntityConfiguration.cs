using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public class AbstractEntityConfiguration
{
    public static void Configure<EntityType>(ModelBuilder modelBuilder)
        where EntityType : AbstractEntity
    {
        modelBuilder.Entity<EntityType>(abstracEntity =>
        {
            abstracEntity.HasKey(e => e.Id);
            abstracEntity.Property(e => e.Version).IsConcurrencyToken();
            abstracEntity.HasQueryFilter(e => !e.IsDeleted);

            abstracEntity
                .HasIndex(e => e.IsDeleted)
                .HasFilter($"{nameof(AbstractEntity.IsDeleted)} = false");
        });
    }
}
