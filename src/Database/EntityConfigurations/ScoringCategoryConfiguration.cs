using BoardGameTracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameTracker.Database.EntityConfigurations;

public static partial class ModelBuilderExtensions
{
    public static void ConfigureScoringCategory(this ModelBuilder modelBuilder)
    {
        AbstractEntityConfiguration.Configure<ScoringCategory>(modelBuilder);
        modelBuilder.Entity<ScoringCategory>(entity =>
        {
            entity
                .HasOne(e => e.BoardGame)
                .WithMany(e => e.ScoringCategories)
                .HasForeignKey(e => e.BoardGameId)
                .IsRequired();

            // Filtered so a soft-deleted category's name can be reused
            entity
                .HasIndex(e => new { e.BoardGameId, e.NormalizedName })
                .IsUnique()
                .HasFilter("is_deleted = false");

            // Not unique: a reorder briefly holds duplicate positions while saving
            entity.HasIndex(e => new { e.BoardGameId, e.Order });
        });
    }

    public static void ConfigureCategoryScore(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryScore>(entity =>
        {
            entity.HasKey(e => new { e.PlayerScoreId, e.ScoringCategoryId });

            entity
                .HasOne(e => e.PlayerScore)
                .WithMany(e => e.CategoryScores)
                .HasForeignKey(e => e.PlayerScoreId)
                .IsRequired();

            entity
                .HasOne(e => e.ScoringCategory)
                .WithMany(e => e.CategoryScores)
                .HasForeignKey(e => e.ScoringCategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Hidden with its soft-deleted player score; kept when the category is deleted with its board game
            entity.HasQueryFilter(e => !e.PlayerScore!.IsDeleted);
        });
    }
}
