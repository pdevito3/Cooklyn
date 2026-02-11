namespace Cooklyn.UnitTests.Domain.Recipes;

using Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;
using Shouldly;

public class CmtTextParserTests
{
    [Fact]
    public void can_parse_title()
    {
        var content = "Basil Pesto\n\nINGREDIENTS\n\n2 cups basil";

        var result = CmtTextParser.Parse(content);

        result.Title.ShouldBe("Basil Pesto");
    }

    [Fact]
    public void can_parse_source_url_with_adapted_from()
    {
        var content = """
            Basil Pesto

            Adapted from http://www.foodnetwork.com/basil-pesto

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Source.ShouldBe("http://www.foodnetwork.com/basil-pesto");
    }

    [Fact]
    public void can_parse_raw_url_as_source()
    {
        var content = """
            Basil Pesto

            https://example.com/recipe

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Source.ShouldBe("https://example.com/recipe");
    }

    [Fact]
    public void can_parse_rating()
    {
        var content = """
            Basil Pesto

            I made this. Rated 4/5

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Rating.ShouldBe(4);
    }

    [Fact]
    public void can_parse_rating_without_made_this()
    {
        var content = """
            Basil Pesto

            Rated 3/5

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Rating.ShouldBe(3);
    }

    [Fact]
    public void can_parse_servings()
    {
        var content = """
            Basil Pesto

            Servings: 6 servings

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Servings.ShouldBe("6 servings");
    }

    [Fact]
    public void can_parse_tags()
    {
        var content = """
            Basil Pesto

            tags: Italian, Sauce, Quick

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Tags.Count.ShouldBe(3);
        result.Tags.ShouldContain("Italian");
        result.Tags.ShouldContain("Sauce");
        result.Tags.ShouldContain("Quick");
    }

    [Fact]
    public void can_parse_ingredients()
    {
        var content = """
            Basil Pesto

            INGREDIENTS

            2 cups packed fresh basil leaves
            2 cloves garlic
            1/4 cup pine nuts

            STEPS

            1) Combine ingredients.
            """;

        var result = CmtTextParser.Parse(content);

        result.IngredientLines.Count.ShouldBe(3);
        result.IngredientLines[0].ShouldBe("2 cups packed fresh basil leaves");
        result.IngredientLines[1].ShouldBe("2 cloves garlic");
        result.IngredientLines[2].ShouldBe("1/4 cup pine nuts");
    }

    [Fact]
    public void can_parse_ingredient_subheaders()
    {
        var content = """
            Biryani

            INGREDIENTS

            Spices for tempering
            2 tablespoons ghee
            1/2 teaspoon cumin seeds

            Chicken and Rice
            1 pound chicken thighs
            1 cup basmati rice

            STEPS

            1) Cook it.
            """;

        var result = CmtTextParser.Parse(content);

        result.IngredientLines.Count.ShouldBe(6);
        result.IngredientLines[0].ShouldBe("Spices for tempering");
        result.IngredientLines[1].ShouldBe("2 tablespoons ghee");
        result.IngredientLines[2].ShouldBe("1/2 teaspoon cumin seeds");
        result.IngredientLines[3].ShouldBe("Chicken and Rice");
        result.IngredientLines[4].ShouldBe("1 pound chicken thighs");
        result.IngredientLines[5].ShouldBe("1 cup basmati rice");
    }

    [Fact]
    public void can_parse_steps_and_strip_numbers()
    {
        var content = """
            Pesto

            INGREDIENTS

            2 cups basil

            STEPS

            1) Combine the basil and garlic in a food processor.
            2) Add oil and process until smooth.
            3) Season with salt and pepper.
            """;

        var result = CmtTextParser.Parse(content);

        result.Steps.ShouldNotBeNull();
        result.Steps.ShouldBe(
            "Combine the basil and garlic in a food processor.\n\n" +
            "Add oil and process until smooth.\n\n" +
            "Season with salt and pepper.");
    }

    [Fact]
    public void can_parse_notes()
    {
        var content = """
            Pesto

            INGREDIENTS

            2 cups basil

            STEPS

            1) Combine ingredients.

            NOTES

            If freezing, drizzle oil over the top.
            Thaw and stir in cheese before serving.
            """;

        var result = CmtTextParser.Parse(content);

        result.Notes.ShouldNotBeNull();
        result.Notes.ShouldBe(
            "If freezing, drizzle oil over the top.\n" +
            "Thaw and stir in cheese before serving.");
    }

    [Fact]
    public void minimal_recipe_has_null_optional_fields()
    {
        var content = """
            Simple Spice Mix

            INGREDIENTS

            1 teaspoon salt
            2 teaspoons paprika

            STEPS

            1) Mix all powders together.
            """;

        var result = CmtTextParser.Parse(content);

        result.Title.ShouldBe("Simple Spice Mix");
        result.Source.ShouldBeNull();
        result.Rating.ShouldBeNull();
        result.Servings.ShouldBeNull();
        result.Notes.ShouldBeNull();
        result.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void can_parse_all_header_fields_together()
    {
        var content = """
            Chicken Biryani

            Adapted from https://example.com/biryani

            tags: Indian, Dinner

            I made this. Rated 5/5

            Servings: 6 servings

            INGREDIENTS

            1 pound chicken

            STEPS

            1) Cook it.
            """;

        var result = CmtTextParser.Parse(content);

        result.Title.ShouldBe("Chicken Biryani");
        result.Source.ShouldBe("https://example.com/biryani");
        result.Tags.ShouldContain("Indian");
        result.Tags.ShouldContain("Dinner");
        result.Rating.ShouldBe(5);
        result.Servings.ShouldBe("6 servings");
    }

    [Fact]
    public void empty_content_returns_empty_title()
    {
        var result = CmtTextParser.Parse("");

        result.Title.ShouldBe("");
    }

    [Fact]
    public void title_only_returns_recipe_with_defaults()
    {
        var result = CmtTextParser.Parse("My Recipe");

        result.Title.ShouldBe("My Recipe");
        result.IngredientLines.ShouldBeEmpty();
        result.Steps.ShouldBeNull();
    }

    [Fact]
    public void i_made_this_without_rating_is_ignored()
    {
        var content = """
            Pesto

            I made this.

            INGREDIENTS

            2 cups basil
            """;

        var result = CmtTextParser.Parse(content);

        result.Rating.ShouldBeNull();
    }

    [Fact]
    public void skips_blank_ingredient_lines()
    {
        var content = """
            Pesto

            INGREDIENTS

            2 cups basil

            1 clove garlic

            STEPS

            1) Mix.
            """;

        var result = CmtTextParser.Parse(content);

        result.IngredientLines.Count.ShouldBe(2);
    }
}
