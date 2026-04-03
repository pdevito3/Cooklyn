namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.ShoppingLists;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("sl");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne<Store>()
            .WithMany()
            .HasForeignKey(e => e.StoreId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.ComplexProperty(e => e.Status, status =>
        {
            status.Property(s => s.Value)
                .HasColumnName("status")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}
