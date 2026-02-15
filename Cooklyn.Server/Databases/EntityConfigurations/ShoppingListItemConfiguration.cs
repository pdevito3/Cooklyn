namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ShoppingLists;
using Domain.StoreSections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("sli");

        builder.Property(e => e.ShoppingListId)
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

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.Property(e => e.SortOrder)
            .IsRequired();

        builder.HasOne<StoreSection>()
            .WithMany()
            .HasForeignKey(e => e.StoreSectionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.RecipeSources)
            .WithOne()
            .HasForeignKey(rs => rs.ShoppingListItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.RecipeSources)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(e => e.ShoppingListId);
    }
}
