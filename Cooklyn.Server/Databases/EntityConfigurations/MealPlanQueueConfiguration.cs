namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.MealPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class MealPlanQueueConfiguration : IEntityTypeConfiguration<MealPlanQueue>
{
    public void Configure(EntityTypeBuilder<MealPlanQueue> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("mpq");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.IsDefault)
            .IsRequired();

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.QueueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}
