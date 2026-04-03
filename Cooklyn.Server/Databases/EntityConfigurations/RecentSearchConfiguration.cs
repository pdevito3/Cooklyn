namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.RecentSearches;
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

        // Efficient recent queries
        builder.HasIndex(e => e.CreatedOn)
            .IsDescending(true);

        // Deduplication lookups
        builder.HasIndex(e => new { e.SearchType, e.SearchText, e.ResourceType, e.ResourceId });
    }
}
