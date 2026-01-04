namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class NutritionInfoConfiguration : IEntityTypeConfiguration<NutritionInfo>
{
    public void Configure(EntityTypeBuilder<NutritionInfo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RecipeId)
            .IsRequired();

        // Macronutrients
        builder.Property(e => e.Calories)
            .HasPrecision(10, 2);

        builder.Property(e => e.TotalFatGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.SaturatedFatGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.TransFatGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.CholesterolMilligrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.SodiumMilligrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.TotalCarbohydratesGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.DietaryFiberGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.TotalSugarsGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.AddedSugarsGrams)
            .HasPrecision(10, 2);

        builder.Property(e => e.ProteinGrams)
            .HasPrecision(10, 2);

        // Micronutrients (percentages)
        builder.Property(e => e.VitaminDPercent)
            .HasPrecision(5, 2);

        builder.Property(e => e.CalciumPercent)
            .HasPrecision(5, 2);

        builder.Property(e => e.IronPercent)
            .HasPrecision(5, 2);

        builder.Property(e => e.PotassiumPercent)
            .HasPrecision(5, 2);

        // Indexes
        builder.HasIndex(e => e.RecipeId)
            .IsUnique();
    }
}
