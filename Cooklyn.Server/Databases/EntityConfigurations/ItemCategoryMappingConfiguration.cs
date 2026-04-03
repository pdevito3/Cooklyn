namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ItemCategoryMappings;
using Domain.StoreSections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ItemCategoryMappingConfiguration : IEntityTypeConfiguration<ItemCategoryMapping>
{
    public void Configure(EntityTypeBuilder<ItemCategoryMapping> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("icm");

        builder.Property(e => e.NormalizedName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.StoreSectionId)
            .IsRequired();

        builder.HasOne<StoreSection>()
            .WithMany()
            .HasForeignKey(e => e.StoreSectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(e => e.Source, source =>
        {
            source.Property(s => s.Value)
                .HasColumnName("source")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Unique normalized name (excluding soft-deleted records)
        builder.HasIndex(e => e.NormalizedName)
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
