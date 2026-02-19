namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.MealPlans;
using Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MealPlanQueueItemConfiguration : IEntityTypeConfiguration<MealPlanQueueItem>
{
    public void Configure(EntityTypeBuilder<MealPlanQueueItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("mpqi");

        builder.Property(e => e.QueueId)
            .IsRequired();

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

        builder.HasIndex(e => e.QueueId);
    }
}
