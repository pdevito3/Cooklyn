namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.StoreSections;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StoreAisleConfiguration : IEntityTypeConfiguration<StoreAisle>
{
    public void Configure(EntityTypeBuilder<StoreAisle> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("sail");

        builder.Property(e => e.StoreId)
            .IsRequired();

        builder.Property(e => e.StoreSectionId)
            .IsRequired();

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.Property(e => e.CustomName)
            .HasMaxLength(100);

        builder.HasOne<StoreSection>()
            .WithMany()
            .HasForeignKey(e => e.StoreSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique section per store
        builder.HasIndex(e => new { e.StoreId, e.StoreSectionId })
            .IsUnique();

        builder.HasIndex(e => e.StoreId);
    }
}
