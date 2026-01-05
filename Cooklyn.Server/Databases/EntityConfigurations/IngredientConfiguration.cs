namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Ingredients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Quantity)
            .HasPrecision(10, 4);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.RecipeId)
            .IsRequired();

        // Value object for unit of measure
        builder.ComplexProperty(e => e.Unit, unit =>
        {
            unit.Property(u => u.Value)
                .HasColumnName("unit")
                .HasMaxLength(50);

            unit.Property(u => u.CustomUnit)
                .HasColumnName("custom_unit")
                .HasMaxLength(100);
        });

        // Indexes
        builder.HasIndex(e => e.RecipeId);
        builder.HasIndex(e => e.SortOrder);
    }
}
