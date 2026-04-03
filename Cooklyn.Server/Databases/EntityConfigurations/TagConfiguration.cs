namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).WithPrefix("tag");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Unique tag name
        builder.HasIndex(e => e.Name)
            .IsUnique();
    }
}
