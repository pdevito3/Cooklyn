namespace Cooklyn.Server.Databases;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class PrefixedIdValueGenerator(string prefix) : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry)
    {
        return $"{prefix}_{Guid.CreateVersion7():N}";
    }
}
