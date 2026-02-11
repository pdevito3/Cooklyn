namespace Cooklyn.UnitTests.Domain.Recipes;

using Cooklyn.Server.Domain.Recipes.Importing.CopyMeThat;
using Shouldly;

public class CmtHtmlParserTests
{
    private const string FullRecipeHtml = """
        <!DOCTYPE html>
        <head>
        <meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
        </head>
        <html>
        <body>
        <div class = "recipe">
        <div id = "name">
            Unbelievable Chicken
        </div>
        <div id = "link">
            Adapted from <a id = "original_link" href = "http://allrecipes.com/Recipe/Unbelievable-Chicken/">http://allrecipes.com/Recipe/Unbelievable-Chicken/</a>
        </div>
        <img class= "recipeImage" src = "images/unbelievable_chicken_l4ddi.jpg" width="120px"/>
        <div id = "categories">
        <span>Tags: </span>
            <span class = "recipeCategory">Dinner</span><span> </span>
            <span class = "recipeCategory">Lunch</span><span> </span>
        </div>
        <div id = "extra_info">
            <span id="made_this"> I made this. </span>
            <span id="rating">Rated <span id = "ratingValue">5</span>/5</span>
        </div>
        <div id = "servings">
            Servings: <a id="recipeYield">6 servings</a>
        </div>
            <ul id = "recipeIngredients">
                <li class= "recipeIngredient">
                    1/4 cup cider vinegar
                </li>
                <li class= "recipeIngredient">
                    3 tablespoons mustard
                </li>
                <li class= "recipeIngredient">
                    6 tablespoons olive oil
                </li>
            </ul>
            <ol id = "recipeInstructions">
                <li class = "instruction" value = "1">
                    butterfly chicken.
                </li>
                <li class = "instruction" value = "2">
                    Preheat grill for high heat.
                </li>
            </ol>
            <ul id = "recipeNotes">
                <li class = "recipeNote">
                   Cube chicken first so marinade absorbs better.
                </li>
            </ul>
        </div>
        </body>
        </html>
        """;

    [Fact]
    public void can_parse_title()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes.Count.ShouldBe(1);
        recipes[0].Title.ShouldBe("Unbelievable Chicken");
    }

    [Fact]
    public void can_parse_source_url()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Source.ShouldBe("http://allrecipes.com/Recipe/Unbelievable-Chicken/");
    }

    [Fact]
    public void can_parse_image_filename()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].ImageFileName.ShouldBe("images/unbelievable_chicken_l4ddi.jpg");
    }

    [Fact]
    public void can_parse_tags()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Tags.Count.ShouldBe(2);
        recipes[0].Tags.ShouldContain("Dinner");
        recipes[0].Tags.ShouldContain("Lunch");
    }

    [Fact]
    public void can_parse_rating()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Rating.ShouldBe(5);
    }

    [Fact]
    public void can_parse_servings()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Servings.ShouldBe("6 servings");
    }

    [Fact]
    public void can_parse_ingredients()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].IngredientLines.Count.ShouldBe(3);
        recipes[0].IngredientLines[0].ShouldBe("1/4 cup cider vinegar");
        recipes[0].IngredientLines[1].ShouldBe("3 tablespoons mustard");
        recipes[0].IngredientLines[2].ShouldBe("6 tablespoons olive oil");
    }

    [Fact]
    public void can_parse_steps()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Steps.ShouldNotBeNull();
        recipes[0].Steps.ShouldContain("butterfly chicken.");
        recipes[0].Steps.ShouldContain("Preheat grill for high heat.");
    }

    [Fact]
    public void can_parse_notes()
    {
        var recipes = CmtHtmlParser.Parse(FullRecipeHtml);

        recipes[0].Notes.ShouldNotBeNull();
        recipes[0].Notes.ShouldContain("Cube chicken first");
    }

    [Fact]
    public void can_parse_ingredient_subheaders()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Sushi Rice</div>
                <ul id = "recipeIngredients">
                    <li class= "recipeIngredient">2 cups sushi rice</li>
                    <div class= "recipeIngredient recipeIngredient_subheader">roll:</div>
                    <li class= "recipeIngredient">cream cheese</li>
                    <li class= "recipeIngredient">smoked salmon</li>
                </ul>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].IngredientLines.Count.ShouldBe(4);
        recipes[0].IngredientLines[0].ShouldBe("2 cups sushi rice");
        recipes[0].IngredientLines[1].ShouldBe("roll:");
        recipes[0].IngredientLines[2].ShouldBe("cream cheese");
        recipes[0].IngredientLines[3].ShouldBe("smoked salmon");
    }

    [Fact]
    public void can_parse_multiple_recipes()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Recipe One</div>
                <ul id = "recipeIngredients">
                    <li class= "recipeIngredient">1 cup flour</li>
                </ul>
            </div>
            <div class = "recipe">
            <div id = "name">Recipe Two</div>
                <ul id = "recipeIngredients">
                    <li class= "recipeIngredient">2 cups sugar</li>
                </ul>
            </div>
            <div class = "recipe">
            <div id = "name">Recipe Three</div>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes.Count.ShouldBe(3);
        recipes[0].Title.ShouldBe("Recipe One");
        recipes[1].Title.ShouldBe("Recipe Two");
        recipes[2].Title.ShouldBe("Recipe Three");
    }

    [Fact]
    public void skips_recipe_without_title()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">   </div>
            </div>
            <div class = "recipe">
            <div id = "name">Valid Recipe</div>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes.Count.ShouldBe(1);
        recipes[0].Title.ShouldBe("Valid Recipe");
    }

    [Fact]
    public void recipe_without_optional_fields_has_null_values()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Simple Recipe</div>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].Source.ShouldBeNull();
        recipes[0].Description.ShouldBeNull();
        recipes[0].Servings.ShouldBeNull();
        recipes[0].Rating.ShouldBeNull();
        recipes[0].ImageFileName.ShouldBeNull();
        recipes[0].Steps.ShouldBeNull();
        recipes[0].Notes.ShouldBeNull();
        recipes[0].Tags.ShouldBeEmpty();
        recipes[0].IngredientLines.ShouldBeEmpty();
    }

    [Fact]
    public void ignores_invalid_rating()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Bad Rating</div>
            <div id = "extra_info">
                <span id="rating">Rated <span id = "ratingValue">0</span>/5</span>
            </div>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].Rating.ShouldBeNull();
    }

    [Fact]
    public void can_parse_description()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Described Recipe</div>
            <div id = "description">A wonderful family recipe passed down through generations.</div>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].Description.ShouldBe("A wonderful family recipe passed down through generations.");
    }

    [Fact]
    public void steps_joined_with_newlines()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Multi Step</div>
            <ol id = "recipeInstructions">
                <li class = "instruction" value = "1">Step one.</li>
                <li class = "instruction" value = "2">Step two.</li>
                <li class = "instruction" value = "3">Step three.</li>
            </ol>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].Steps.ShouldBe("Step one.\nStep two.\nStep three.");
    }

    [Fact]
    public void notes_joined_with_newlines()
    {
        var html = """
            <body>
            <div class = "recipe">
            <div id = "name">Multi Note</div>
            <ul id = "recipeNotes">
                <li class = "recipeNote">Note one.</li>
                <li class = "recipeNote">Note two.</li>
            </ul>
            </div>
            </body>
            """;

        var recipes = CmtHtmlParser.Parse(html);

        recipes[0].Notes.ShouldBe("Note one.\nNote two.");
    }
}
