namespace Cooklyn.Server.Databases.EntityConfigurations;

using Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class RecipeFlagEntryConfiguration : IEntityTypeConfiguration<RecipeFlagEntry>
{
    public void Configure(EntityTypeBuilder<RecipeFlagEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RecipeId)
            .IsRequired();

        // Value object for recipe flag
        builder.ComplexProperty(e => e.Flag, flag =>
        {
            flag.Property(f => f.Value)
                .HasColumnName("flag")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Indexes
        builder.HasIndex(e => e.RecipeId);
    }
}
