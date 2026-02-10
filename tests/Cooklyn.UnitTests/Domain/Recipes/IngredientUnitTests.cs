namespace Cooklyn.UnitTests.Domain.Recipes;

using Cooklyn.Server.Domain.Recipes;
using Shouldly;

public class IngredientUnitTests
{
    // --- Construction ---

    [Fact]
    public void can_create_from_canonical_name()
    {
        var unit = new IngredientUnit("Cup");

        unit.Value.ShouldBe("Cup");
        unit.Abbreviation.ShouldBe("c");
        unit.PluralName.ShouldBe("Cups");
        unit.IsCustom.ShouldBeFalse();
    }

    [Fact]
    public void can_create_from_canonical_name_case_insensitively()
    {
        var unit = new IngredientUnit("cup");

        unit.Value.ShouldBe("Cup");
    }

    [Fact]
    public void can_create_empty_unit_from_empty_string()
    {
        var unit = new IngredientUnit(string.Empty);

        unit.Value.ShouldBe(string.Empty);
        unit.Abbreviation.ShouldBe(string.Empty);
        unit.IsCustom.ShouldBeFalse();
    }

    [Fact]
    public void can_create_empty_unit_from_whitespace()
    {
        var unit = new IngredientUnit("   ");

        unit.Value.ShouldBe(string.Empty);
    }

    [Fact]
    public void unknown_unit_becomes_custom()
    {
        var unit = new IngredientUnit("handful");

        unit.Value.ShouldBe("Custom");
        unit.IsCustom.ShouldBeTrue();
    }

    // --- TryParse: basic aliases ---

    [Theory]
    [InlineData("tbsp", "Tablespoon")]
    [InlineData("tsp", "Teaspoon")]
    [InlineData("cups", "Cup")]
    [InlineData("oz", "Ounce")]
    [InlineData("lbs", "Pound")]
    [InlineData("g", "Gram")]
    [InlineData("kg", "Kilogram")]
    [InlineData("ml", "Milliliter")]
    [InlineData("fl oz", "FluidOunce")]
    [InlineData("pkg.", "Package")]
    public void can_parse_common_abbreviations(string input, string expected)
    {
        var result = IngredientUnit.TryParse(input);

        result.ShouldNotBeNull();
        result.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("Tablespoon")]
    [InlineData("tablespoon")]
    [InlineData("TABLESPOON")]
    public void can_parse_full_name_case_insensitively(string input)
    {
        var result = IngredientUnit.TryParse(input);

        result.ShouldNotBeNull();
        result.Value.ShouldBe("Tablespoon");
    }

    [Theory]
    [InlineData("Cups", "Cup")]
    [InlineData("Tablespoons", "Tablespoon")]
    [InlineData("ounces", "Ounce")]
    public void can_parse_plural_names(string input, string expected)
    {
        var result = IngredientUnit.TryParse(input);

        result.ShouldNotBeNull();
        result.Value.ShouldBe(expected);
    }

    // --- TryParse: case-sensitive T vs t ---

    [Fact]
    public void can_parse_uppercase_t_as_tablespoon()
    {
        var result = IngredientUnit.TryParse("T");

        result.ShouldNotBeNull();
        result.Value.ShouldBe("Tablespoon");
    }

    [Fact]
    public void can_parse_lowercase_t_as_teaspoon()
    {
        var result = IngredientUnit.TryParse("t");

        result.ShouldNotBeNull();
        result.Value.ShouldBe("Teaspoon");
    }

    // --- TryParse: null/empty/unknown ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void returns_null_for_empty_or_null_input(string? input)
    {
        var result = IngredientUnit.TryParse(input!);

        result.ShouldBeNull();
    }

    [Fact]
    public void returns_null_for_unknown_unit()
    {
        var result = IngredientUnit.TryParse("bushel");

        result.ShouldBeNull();
    }

    // --- ListNames / GetAll ---

    [Fact]
    public void list_names_excludes_custom()
    {
        var names = IngredientUnit.ListNames();

        names.ShouldNotContain("Custom");
        names.ShouldContain("Cup");
        names.ShouldContain("Tablespoon");
        names.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void get_all_excludes_custom()
    {
        var units = IngredientUnit.GetAll();

        units.ShouldAllBe(u => !u.IsCustom);
        units.Count.ShouldBe(IngredientUnit.ListNames().Count);
    }

    // --- Implicit conversion ---

    [Fact]
    public void can_implicitly_convert_to_string()
    {
        var unit = new IngredientUnit("Gram");
        string value = unit;

        value.ShouldBe("Gram");
    }
}
