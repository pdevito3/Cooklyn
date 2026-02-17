namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ItemCollections;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StoreDefaultCollectionConfiguration : IEntityTypeConfiguration<StoreDefaultCollection>
{
    public void Configure(EntityTypeBuilder<StoreDefaultCollection> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("sdc");

        builder.Property(e => e.StoreId)
            .IsRequired();

        builder.Property(e => e.ItemCollectionId)
            .IsRequired();

        builder.HasOne(e => e.ItemCollection)
            .WithMany()
            .HasForeignKey(e => e.ItemCollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique collection per store (filtered for soft delete)
        builder.HasIndex(e => new { e.StoreId, e.ItemCollectionId })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(e => e.StoreId);
    }
}
