namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RecipeId)
            .IsRequired();

        builder.Property(e => e.TagId)
            .IsRequired();

        builder.HasOne(e => e.Tag)
            .WithMany()
            .HasForeignKey(e => e.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint to prevent duplicate tag assignments
        builder.HasIndex(e => new { e.RecipeId, e.TagId })
            .IsUnique();

        builder.HasIndex(e => e.RecipeId);
        builder.HasIndex(e => e.TagId);
    }
}
