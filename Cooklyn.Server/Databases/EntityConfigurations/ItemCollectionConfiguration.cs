namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ItemCollections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ItemCollectionConfiguration : IEntityTypeConfiguration<ItemCollection>
{
    public void Configure(EntityTypeBuilder<ItemCollection> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("ic");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.ItemCollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Unique collection name (excluding soft-deleted records)
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
