namespace Cooklyn.UnitTests.Services;

using Cooklyn.Server.Services;
using Shouldly;

public class ItemNameNormalizerTests
{
    [Theory]
    [InlineData("Chicken", "chicken")]
    [InlineData("  FLOUR  ", "flour")]
    [InlineData("Fresh Organic Chicken Breast", "chicken breast")]
    [InlineData("ORGANIC DICED TOMATOES", "tomato")]
    [InlineData("Large Boneless Skinless Chicken Thigh", "chicken thigh")]
    public void normalize_lowercases_trims_and_strips_qualifiers(string input, string expected)
    {
        ItemNameNormalizer.Normalize(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData("tomatoes", "tomato")]
    [InlineData("berries", "berry")]
    [InlineData("potatoes", "potato")]
    [InlineData("eggs", "egg")]
    [InlineData("glass", "glass")]
    [InlineData("asparagus", "asparagus")]
    public void normalize_singularizes_words(string input, string expected)
    {
        ItemNameNormalizer.Normalize(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData("butter (unsalted)", "butter")]
    [InlineData("chicken breast (bone-in)", "chicken breast")]
    [InlineData("milk (2%)", "milk")]
    public void normalize_strips_parentheticals(string input, string expected)
    {
        ItemNameNormalizer.Normalize(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    public void normalize_handles_empty_and_null(string? input, string expected)
    {
        ItemNameNormalizer.Normalize(input!).ShouldBe(expected);
    }

    [Fact]
    public void normalize_keeps_all_words_when_all_are_qualifiers()
    {
        // "fresh" and "frozen" are both qualifiers, so keep originals
        var result = ItemNameNormalizer.Normalize("fresh frozen");
        result.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData("chicken breast", new[] { "chicken", "breast" })]
    [InlineData("olive oil", new[] { "olive", "oil" })]
    [InlineData("salt", new[] { "salt" })]
    public void extract_tokens_splits_on_spaces(string input, string[] expected)
    {
        ItemNameNormalizer.ExtractTokens(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void extract_tokens_returns_empty_for_blank_input(string? input)
    {
        ItemNameNormalizer.ExtractTokens(input!).ShouldBeEmpty();
    }

    [Fact]
    public void normalize_handles_compound_qualifier_removal()
    {
        // "finely chopped garlic" -> strip "finely" and "chopped" qualifiers -> "garlic"
        ItemNameNormalizer.Normalize("finely chopped garlic").ShouldBe("garlic");
    }

    [Fact]
    public void normalize_multi_word_items_preserve_meaningful_words()
    {
        // "dried red pepper flake" -> "dried" is qualifier, "red" is not
        // Result should be "red pepper flake"
        ItemNameNormalizer.Normalize("dried red pepper flake").ShouldBe("red pepper flake");
    }
}
