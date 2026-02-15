namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Domain.ShoppingLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ShoppingListItemRecipeSourceConfiguration : IEntityTypeConfiguration<ShoppingListItemRecipeSource>
{
    public void Configure(EntityTypeBuilder<ShoppingListItemRecipeSource> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("slirs");

        builder.Property(e => e.ShoppingListItemId)
            .IsRequired();

        builder.Property(e => e.RecipeId)
            .IsRequired();

        builder.Property(e => e.OriginalQuantity)
            .HasPrecision(10, 4);

        builder.Property(e => e.OriginalUnit)
            .HasMaxLength(50);

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(e => e.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique recipe source per item
        builder.HasIndex(e => new { e.ShoppingListItemId, e.RecipeId })
            .IsUnique();

        builder.HasIndex(e => e.ShoppingListItemId);
    }
}
