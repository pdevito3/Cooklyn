namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(5000);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(2000);

        builder.Property(e => e.Source)
            .HasMaxLength(2000);

        builder.Property(e => e.Steps);

        builder.Property(e => e.Notes)
            .HasMaxLength(10000);

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Value objects
        builder.ComplexProperty(e => e.Rating, rating =>
        {
            rating.Property(r => r.Value)
                .HasColumnName("rating")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Navigation properties with private backing fields
        builder.HasMany(e => e.Ingredients)
            .WithOne()
            .HasForeignKey(i => i.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Ingredients)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.RecipeTags)
            .WithOne(rt => rt.Recipe)
            .HasForeignKey(rt => rt.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.RecipeTags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Flags)
            .WithOne()
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Flags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // One-to-one with NutritionInfo
        builder.HasOne(e => e.NutritionInfo)
            .WithOne()
            .HasForeignKey<NutritionInfo>(n => n.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.Title);
        builder.HasIndex(e => e.IsFavorite);
    }
}
