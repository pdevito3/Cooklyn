namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.SavedFilters;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
{
    public void Configure(EntityTypeBuilder<SavedFilter> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("sf");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Context)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.FilterStateJson)
            .IsRequired();

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique filter name per tenant and context
        builder.HasIndex(e => new { e.TenantId, e.Name, e.Context })
            .IsUnique();

        builder.HasIndex(e => new { e.TenantId, e.Context });
    }
}
