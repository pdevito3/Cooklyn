namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.RecentSearches;
using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RecentSearchConfiguration : IEntityTypeConfiguration<RecentSearch>
{
    public void Configure(EntityTypeBuilder<RecentSearch> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("rs");

        builder.Property(e => e.SearchType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.SearchText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ResourceType)
            .HasMaxLength(50);

        builder.Property(e => e.ResourceId)
            .HasMaxLength(100);

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Efficient recent queries
        builder.HasIndex(e => new { e.TenantId, e.CreatedOn })
            .IsDescending(false, true);

        // Deduplication lookups
        builder.HasIndex(e => new { e.TenantId, e.SearchType, e.SearchText, e.ResourceType, e.ResourceId });
    }
}
