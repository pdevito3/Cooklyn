namespace Cooklyn.UnitTests.Domain.ItemCategoryMappings;

using Cooklyn.Server.Domain.ItemCategoryMappings;
using Cooklyn.Server.Exceptions;
using Shouldly;

public class MappingSourceTests
{
    [Theory]
    [InlineData("Seed")]
    [InlineData("User")]
    [InlineData("System")]
    public void can_create_with_valid_values(string value)
    {
        var source = MappingSource.Of(value);
        source.Value.ShouldBe(value);
    }

    [Fact]
    public void invalid_value_throws_validation_exception()
    {
        Should.Throw<ValidationException>(() => MappingSource.Of("Invalid"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void empty_or_null_throws_validation_exception(string? value)
    {
        Should.Throw<ValidationException>(() => new MappingSource(value!));
    }

    [Fact]
    public void factory_methods_create_correct_values()
    {
        MappingSource.Seed().Value.ShouldBe("Seed");
        MappingSource.User().Value.ShouldBe("User");
        MappingSource.System().Value.ShouldBe("System");
    }

    [Fact]
    public void implicit_string_conversion_works()
    {
        string value = MappingSource.Seed();
        value.ShouldBe("Seed");
    }

    [Fact]
    public void case_insensitive_parsing()
    {
        var source = MappingSource.Of("seed");
        source.Value.ShouldBe("Seed");
    }

    [Fact]
    public void equality_by_value()
    {
        var a = MappingSource.Seed();
        var b = MappingSource.Seed();
        a.ShouldBe(b);
    }
}
