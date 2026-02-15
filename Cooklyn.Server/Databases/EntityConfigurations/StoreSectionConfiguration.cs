namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.StoreSections;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StoreSectionConfiguration : IEntityTypeConfiguration<StoreSection>
{
    public void Configure(EntityTypeBuilder<StoreSection> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("ssec");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique section name per tenant (excluding soft-deleted records)
        builder.HasIndex(e => new { e.TenantId, e.Name })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(e => e.TenantId);
    }
}
