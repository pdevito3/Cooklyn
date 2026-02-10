namespace Cooklyn.Server.Databases;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<string> WithPrefix(this PropertyBuilder<string> builder, string prefix)
    {
        return builder.HasValueGenerator((_, _) => new PrefixedIdValueGenerator(prefix));
    }
}
