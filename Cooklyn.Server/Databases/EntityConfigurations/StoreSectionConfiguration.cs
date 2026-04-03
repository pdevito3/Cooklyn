namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.StoreSections;
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

        // Unique section name (excluding soft-deleted records)
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
