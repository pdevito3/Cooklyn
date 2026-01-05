namespace Cooklyn.Server.Domain.Ingredients.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Extensions;

[ApiController]
[Route("api/v{v:apiVersion}/recipes/{recipeId:guid}/[controller]")]
[ApiVersion("1.0")]
public sealed class IngredientsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single ingredient by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetIngredient")]
    [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IngredientDto>> GetIngredient(Guid recipeId, Guid id)
    {
        var query = new GetIngredient.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets all ingredients for a recipe.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetIngredientList")]
    [ProducesResponseType(typeof(PagedList<IngredientDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<IngredientDto>>> GetIngredientList(
        Guid recipeId,
        [FromQuery] IngredientParametersDto parameters)
    {
        var query = new GetIngredientList.Query(recipeId, parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Adds a new ingredient to a recipe.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddIngredient")]
    [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IngredientDto>> AddIngredient(
        Guid recipeId,
        [FromBody] IngredientForCreationDto dto)
    {
        var command = new AddIngredient.Command(recipeId, dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetIngredient",
            new { recipeId, id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing ingredient.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateIngredient")]
    [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IngredientDto>> UpdateIngredient(
        Guid recipeId,
        Guid id,
        [FromBody] IngredientForUpdateDto dto)
    {
        var command = new UpdateIngredient.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an ingredient.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteIngredient")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteIngredient(Guid recipeId, Guid id)
    {
        var command = new DeleteIngredient.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Reorders ingredients for a recipe.
    /// </summary>
    [Authorize]
    [HttpPut("reorder", Name = "ReorderIngredients")]
    [ProducesResponseType(typeof(IReadOnlyList<IngredientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<IngredientDto>>> ReorderIngredients(
        Guid recipeId,
        [FromBody] ReorderIngredientsDto dto)
    {
        var command = new ReorderIngredients.Command(recipeId, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
