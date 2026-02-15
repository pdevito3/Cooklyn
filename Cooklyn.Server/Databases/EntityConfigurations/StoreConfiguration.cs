namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Stores;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("str");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.StoreAisles)
            .WithOne()
            .HasForeignKey(a => a.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.StoreAisles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.StoreDefaultCollections)
            .WithOne()
            .HasForeignKey(sdc => sdc.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.StoreDefaultCollections)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Unique store name per tenant (excluding soft-deleted records)
        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(e => e.TenantId);
    }
}
