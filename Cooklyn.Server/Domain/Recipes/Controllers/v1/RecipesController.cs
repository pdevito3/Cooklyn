namespace Cooklyn.Server.Domain.Recipes.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Extensions;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class RecipesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single Recipe by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id}", Name = "GetRecipe")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> GetRecipe(string id)
    {
        var query = new GetRecipe.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of Recipes.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetRecipeList")]
    [ProducesResponseType(typeof(PagedList<RecipeSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<RecipeSummaryDto>>> GetRecipeList(
        [FromQuery] RecipeParametersDto parameters)
    {
        var query = new GetRecipeList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new Recipe.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddRecipe")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> AddRecipe(
        [FromBody] RecipeForCreationDto dto)
    {
        var command = new AddRecipe.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetRecipe",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing Recipe.
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = "UpdateRecipe")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> UpdateRecipe(
        string id,
        [FromBody] RecipeForUpdateDto dto)
    {
        var command = new UpdateRecipe.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a Recipe.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}", Name = "DeleteRecipe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRecipe(string id)
    {
        var command = new DeleteRecipe.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Toggles the favorite status of a Recipe.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/toggle-favorite", Name = "ToggleRecipeFavorite")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> ToggleFavorite(string id)
    {
        var command = new ToggleRecipeFavorite.Command(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates the rating of a Recipe.
    /// </summary>
    [Authorize]
    [HttpPut("{id}/rating", Name = "UpdateRecipeRating")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> UpdateRating(
        string id,
        [FromBody] UpdateRecipeRatingDto dto)
    {
        var command = new UpdateRecipeRating.Command(id, dto.Rating);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates the tags of a Recipe.
    /// </summary>
    [Authorize]
    [HttpPut("{id}/tags", Name = "UpdateRecipeTags")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> UpdateTags(
        string id,
        [FromBody] UpdateRecipeTagsDto dto)
    {
        var command = new UpdateRecipeTags.Command(id, dto.TagIds);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates the flags of a Recipe.
    /// </summary>
    [Authorize]
    [HttpPut("{id}/flags", Name = "UpdateRecipeFlags")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> UpdateFlags(
        string id,
        [FromBody] UpdateRecipeFlagsDto dto)
    {
        var command = new UpdateRecipeFlags.Command(id, dto.Flags);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates the ingredients of a Recipe.
    /// </summary>
    [Authorize]
    [HttpPut("{id}/ingredients", Name = "UpdateRecipeIngredients")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> UpdateIngredients(
        string id,
        [FromBody] UpdateIngredientsDto dto)
    {
        var command = new UpdateRecipeIngredients.Command(id, dto.Ingredients);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Parses free-text ingredients into structured data.
    /// </summary>
    [Authorize]
    [HttpPost("parse-ingredients", Name = "ParseIngredients")]
    [ProducesResponseType(typeof(IReadOnlyList<IngredientForCreationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<IngredientForCreationDto>>> ParseIngredients(
        [FromBody] ParseIngredientsRequestDto dto)
    {
        var command = new ParseIngredients.Command(dto.Text);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Imports a recipe from a URL and returns a preview.
    /// </summary>
    [Authorize]
    [HttpPost("import/preview", Name = "ImportRecipePreview")]
    [ProducesResponseType(typeof(ImportRecipePreviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportRecipePreviewDto>> ImportPreview(
        [FromBody] ImportRecipePreviewRequestDto dto)
    {
        var command = new ImportRecipePreview.Command(dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Uploads a recipe image from an external URL.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/image-from-url", Name = "UploadRecipeImageFromUrl")]
    [ProducesResponseType(typeof(RecipeImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeImageDto>> UploadImageFromUrl(
        string id,
        [FromBody] UploadImageFromUrlRequestDto dto)
    {
        var command = new UploadRecipeImageFromUrl.Command(id, dto.ImageUrl);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Uploads an image for a Recipe.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/image", Name = "UploadRecipeImage")]
    [ProducesResponseType(typeof(RecipeImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeImageDto>> UploadImage(
        string id,
        IFormFile file)
    {
        var command = new UploadRecipeImage.Command(id, file);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes the image of a Recipe.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}/image", Name = "DeleteRecipeImage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteImage(string id)
    {
        var command = new DeleteRecipeImage.Command(id);
        await mediator.Send(command);
        return NoContent();
    }
}
