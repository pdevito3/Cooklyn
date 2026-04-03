namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.MealPlans;
using Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MealPlanEntryConfiguration : IEntityTypeConfiguration<MealPlanEntry>
{
    public void Configure(EntityTypeBuilder<MealPlanEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("mpe");

        builder.Property(e => e.Date)
            .IsRequired();

        builder.ComplexProperty(e => e.EntryType, entryType =>
        {
            entryType.Property(t => t.Value)
                .HasColumnName("entry_type")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(e => e.RecipeId);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Scale)
            .HasPrecision(5, 2)
            .HasDefaultValue(1.0m);

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(e => e.RecipeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.RecipeId);
    }
}
