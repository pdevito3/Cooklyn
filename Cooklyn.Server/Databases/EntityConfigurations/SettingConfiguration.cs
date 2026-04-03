namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.Property(e => e.Key)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(e => e.Key)
            .IsUnique();

        builder.Property(e => e.Value)
            .HasMaxLength(1024);
    }
}
