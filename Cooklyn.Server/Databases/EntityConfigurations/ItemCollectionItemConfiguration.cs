namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ItemCollections;
using Domain.StoreSections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ItemCollectionItemConfiguration : IEntityTypeConfiguration<ItemCollectionItem>
{
    public void Configure(EntityTypeBuilder<ItemCollectionItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("ici");

        builder.Property(e => e.ItemCollectionId)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.Quantity)
            .HasPrecision(10, 4);

        builder.ComplexProperty(e => e.Unit, unit =>
        {
            unit.Property(u => u.Value)
                .HasColumnName("unit")
                .HasMaxLength(50);
        });

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.HasOne<StoreSection>()
            .WithMany()
            .HasForeignKey(e => e.StoreSectionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.ItemCollectionId);
    }
}
