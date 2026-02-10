namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("ing");

        builder.Property(e => e.RecipeId)
            .IsRequired();

        builder.Property(e => e.RawText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Name)
            .HasMaxLength(300);

        builder.Property(e => e.Amount)
            .HasPrecision(10, 4);

        builder.Property(e => e.AmountText)
            .HasMaxLength(50);

        builder.ComplexProperty(e => e.Unit, unit =>
        {
            unit.Property(u => u.Value)
                .HasColumnName("unit")
                .HasMaxLength(50);
        });

        builder.Ignore(e => e.HasUnit);

        builder.Property(e => e.CustomUnit)
            .HasMaxLength(100);

        builder.Property(e => e.GroupName)
            .HasMaxLength(200);

        builder.Property(e => e.SortOrder)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.RecipeId);
    }
}
