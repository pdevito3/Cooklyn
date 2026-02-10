namespace Cooklyn.UnitTests.Domain.Recipes;

using Cooklyn.Server.Domain.Recipes;
using Cooklyn.SharedTestHelpers;
using Shouldly;

public class IngredientParserTests
{
    private static readonly string RecipeId = IdGenerator.Recipe();

    // --- Single line: amount parsing ---

    [Fact]
    public void can_parse_integer_amount()
    {
        var result = Ingredient.Parse("2 cups flour", RecipeId, 0);

        result.Amount.ShouldBe(2m);
        result.AmountText.ShouldBe("2");
    }

    [Fact]
    public void can_parse_decimal_amount()
    {
        var result = Ingredient.Parse("1.5 cups flour", RecipeId, 0);

        result.Amount.ShouldBe(1.5m);
        result.AmountText.ShouldBe("1.5");
    }

    [Fact]
    public void can_parse_simple_fraction()
    {
        var result = Ingredient.Parse("1/2 cup butter", RecipeId, 0);

        result.Amount.ShouldBe(0.5m);
        result.AmountText.ShouldBe("1/2");
    }

    [Fact]
    public void can_parse_mixed_fraction()
    {
        var result = Ingredient.Parse("1 1/2 tsp salt", RecipeId, 0);

        result.Amount.ShouldBe(1.5m);
        result.AmountText.ShouldBe("1 1/2");
    }

    // --- Single line: unit parsing ---

    [Fact]
    public void can_parse_abbreviation_unit()
    {
        var result = Ingredient.Parse("2 tbsp olive oil", RecipeId, 0);

        result.Unit.Value.ShouldBe("Tablespoon");
        result.Name.ShouldBe("olive oil");
    }

    [Fact]
    public void can_parse_full_name_unit()
    {
        var result = Ingredient.Parse("1 tablespoon honey", RecipeId, 0);

        result.Unit.Value.ShouldBe("Tablespoon");
        result.Name.ShouldBe("honey");
    }

    [Fact]
    public void can_parse_two_word_unit()
    {
        var result = Ingredient.Parse("4 fl oz cream", RecipeId, 0);

        result.Unit.Value.ShouldBe("FluidOunce");
        result.Name.ShouldBe("cream");
    }

    [Fact]
    public void can_parse_unit_with_trailing_period()
    {
        var result = Ingredient.Parse("2 tsp. vanilla", RecipeId, 0);

        result.Unit.Value.ShouldBe("Teaspoon");
        result.Name.ShouldBe("vanilla");
    }

    // --- Case-sensitive T / t ---

    [Fact]
    public void can_parse_uppercase_t_as_tablespoon_in_line()
    {
        var result = Ingredient.Parse("2 T butter", RecipeId, 0);

        result.Amount.ShouldBe(2m);
        result.Unit.Value.ShouldBe("Tablespoon");
        result.Name.ShouldBe("butter");
    }

    [Fact]
    public void can_parse_lowercase_t_as_teaspoon_in_line()
    {
        var result = Ingredient.Parse("1 t vanilla", RecipeId, 0);

        result.Amount.ShouldBe(1m);
        result.Unit.Value.ShouldBe("Teaspoon");
        result.Name.ShouldBe("vanilla");
    }

    // --- No unit ---

    [Fact]
    public void can_parse_amount_without_unit()
    {
        var result = Ingredient.Parse("3 large eggs", RecipeId, 0);

        result.Amount.ShouldBe(3m);
        result.HasUnit.ShouldBeFalse();
        result.Name.ShouldBe("large eggs");
    }

    [Fact]
    public void can_parse_name_only_without_amount()
    {
        var result = Ingredient.Parse("salt and pepper to taste", RecipeId, 0);

        result.Amount.ShouldBeNull();
        result.HasUnit.ShouldBeFalse();
        result.Name.ShouldBe("salt and pepper to taste");
    }

    // --- RawText preservation ---

    [Fact]
    public void preserves_raw_text()
    {
        var result = Ingredient.Parse("  2 cups flour  ", RecipeId, 0);

        result.RawText.ShouldBe("2 cups flour");
    }

    // --- GroupName ---

    [Fact]
    public void can_assign_group_name_to_parsed_ingredient()
    {
        var result = Ingredient.Parse("2 cups flour", RecipeId, 0, "Biscuit");

        result.GroupName.ShouldBe("Biscuit");
    }

    [Fact]
    public void default_group_name_is_null()
    {
        var result = Ingredient.Parse("2 cups flour", RecipeId, 0);

        result.GroupName.ShouldBeNull();
    }

    // --- ParseAll: multi-line ---

    [Fact]
    public void can_parse_multiple_lines()
    {
        var text = "2 cups flour\n1 tsp salt\n3 large eggs";

        var results = Ingredient.ParseAll(text, RecipeId);

        results.Count.ShouldBe(3);
        results[0].Name.ShouldBe("flour");
        results[1].Name.ShouldBe("salt");
        results[2].Name.ShouldBe("large eggs");
    }

    [Fact]
    public void can_skip_blank_lines()
    {
        var text = "2 cups flour\n\n\n1 tsp salt";

        var results = Ingredient.ParseAll(text, RecipeId);

        results.Count.ShouldBe(2);
    }

    [Fact]
    public void can_assign_sequential_sort_orders()
    {
        var text = "2 cups flour\n1 tsp salt\n3 eggs";

        var results = Ingredient.ParseAll(text, RecipeId);

        results[0].SortOrder.ShouldBe(0);
        results[1].SortOrder.ShouldBe(1);
        results[2].SortOrder.ShouldBe(2);
    }

    [Fact]
    public void returns_empty_for_null_or_whitespace()
    {
        Ingredient.ParseAll("", RecipeId).Count.ShouldBe(0);
        Ingredient.ParseAll("   ", RecipeId).Count.ShouldBe(0);
        Ingredient.ParseAll(null!, RecipeId).Count.ShouldBe(0);
    }

    // --- ParseAll: group headers ---

    [Fact]
    public void can_parse_group_headers()
    {
        var text = "Biscuit:\n2 cups flour\n1 tsp salt\nGravy:\n2 T butter";

        var results = Ingredient.ParseAll(text, RecipeId);

        results.Count.ShouldBe(3);
        results[0].GroupName.ShouldBe("Biscuit");
        results[0].Name.ShouldBe("flour");
        results[1].GroupName.ShouldBe("Biscuit");
        results[2].GroupName.ShouldBe("Gravy");
        results[2].Name.ShouldBe("butter");
    }

    [Fact]
    public void can_parse_ingredients_before_any_group()
    {
        var text = "3 eggs\nBiscuit:\n2 cups flour";

        var results = Ingredient.ParseAll(text, RecipeId);

        results.Count.ShouldBe(2);
        results[0].GroupName.ShouldBeNull();
        results[0].Name.ShouldBe("eggs");
        results[1].GroupName.ShouldBe("Biscuit");
    }

    [Fact]
    public void does_not_treat_digit_prefixed_colon_as_group()
    {
        var text = "2: cups flour";

        var results = Ingredient.ParseAll(text, RecipeId);

        // "2:" should not be a group header — it starts with a digit
        results.Count.ShouldBe(1);
        results[0].GroupName.ShouldBeNull();
    }

    [Fact]
    public void group_header_is_not_stored_as_ingredient()
    {
        var text = "Biscuit:\n2 cups flour";

        var results = Ingredient.ParseAll(text, RecipeId);

        results.Count.ShouldBe(1);
        results[0].RawText.ShouldBe("2 cups flour");
    }

    // --- Create / Update ---

    [Fact]
    public void can_create_ingredient_from_model()
    {
        var forCreation = new Server.Domain.Recipes.Models.IngredientForCreation
        {
            RecipeId = RecipeId,
            RawText = "2 cups flour",
            Name = "flour",
            Amount = 2m,
            AmountText = "2",
            Unit = "Cup",
            GroupName = "Dry",
            SortOrder = 0
        };

        var ingredient = Ingredient.Create(forCreation);

        ingredient.RecipeId.ShouldBe(RecipeId);
        ingredient.Unit.Value.ShouldBe("Cup");
        ingredient.GroupName.ShouldBe("Dry");
    }

    [Fact]
    public void can_update_ingredient_from_model()
    {
        var ingredient = Ingredient.Parse("2 cups flour", RecipeId, 0);

        var forUpdate = new Server.Domain.Recipes.Models.IngredientForUpdate
        {
            RawText = "3 tbsp sugar",
            Name = "sugar",
            Amount = 3m,
            AmountText = "3",
            Unit = "Tablespoon",
            SortOrder = 1
        };

        ingredient.Update(forUpdate);

        ingredient.RawText.ShouldBe("3 tbsp sugar");
        ingredient.Name.ShouldBe("sugar");
        ingredient.Amount.ShouldBe(3m);
        ingredient.Unit.Value.ShouldBe("Tablespoon");
        ingredient.SortOrder.ShouldBe(1);
    }

    [Fact]
    public void create_wraps_null_unit_as_empty()
    {
        var forCreation = new Server.Domain.Recipes.Models.IngredientForCreation
        {
            RecipeId = RecipeId,
            RawText = "3 eggs",
            Name = "eggs",
            Amount = 3m,
            AmountText = "3",
            Unit = null,
            SortOrder = 0
        };

        var ingredient = Ingredient.Create(forCreation);

        ingredient.HasUnit.ShouldBeFalse();
        ingredient.Unit.Value.ShouldBe(string.Empty);
    }
}
