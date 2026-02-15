namespace Cooklyn.Server.Domain.ShoppingLists.Controllers.v1;

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
public sealed class ShoppingListsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single ShoppingList by ID with all items.
    /// </summary>
    [Authorize]
    [HttpGet("{id}", Name = "GetShoppingList")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> GetShoppingList(string id)
    {
        var query = new GetShoppingList.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of ShoppingList summaries.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetShoppingListList")]
    [ProducesResponseType(typeof(PagedList<ShoppingListSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<ShoppingListSummaryDto>>> GetShoppingListList(
        [FromQuery] ShoppingListParametersDto parameters)
    {
        var query = new GetShoppingListList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddShoppingList")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> AddShoppingList(
        [FromBody] ShoppingListForCreationDto dto)
    {
        var command = new AddShoppingList.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetShoppingList",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = "UpdateShoppingList")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> UpdateShoppingList(
        string id,
        [FromBody] ShoppingListForUpdateDto dto)
    {
        var command = new UpdateShoppingList.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a ShoppingList.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}", Name = "DeleteShoppingList")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteShoppingList(string id)
    {
        var command = new DeleteShoppingList.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Marks a ShoppingList as completed.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/complete", Name = "CompleteShoppingList")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> CompleteShoppingList(string id)
    {
        var command = new CompleteShoppingList.Command(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Reopens a completed ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/reopen", Name = "ReopenShoppingList")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> ReopenShoppingList(string id)
    {
        var command = new ReopenShoppingList.Command(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Adds an item to a ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/items", Name = "AddShoppingListItem")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> AddShoppingListItem(
        string id,
        [FromBody] ShoppingListItemForCreationDto dto)
    {
        var command = new AddShoppingListItem.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an item in a ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPut("{id}/items/{itemId}", Name = "UpdateShoppingListItem")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> UpdateShoppingListItem(
        string id,
        string itemId,
        [FromBody] ShoppingListItemForUpdateDto dto)
    {
        var command = new UpdateShoppingListItem.Command(id, itemId, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an item from a ShoppingList.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}/items/{itemId}", Name = "DeleteShoppingListItem")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> DeleteShoppingListItem(
        string id,
        string itemId)
    {
        var command = new DeleteShoppingListItem.Command(id, itemId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Toggles the checked state of a ShoppingList item.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/items/{itemId}/toggle-check", Name = "ToggleShoppingListItemCheck")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> ToggleShoppingListItemCheck(
        string id,
        string itemId)
    {
        var command = new ToggleShoppingListItemCheck.Command(id, itemId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Removes all checked items from a ShoppingList.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/remove-checked", Name = "RemoveCheckedItems")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> RemoveCheckedItems(string id)
    {
        var command = new RemoveCheckedItems.Command(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Adds items from a recipe to a ShoppingList with quantity merging.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/add-from-recipe", Name = "AddItemsFromRecipe")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> AddItemsFromRecipe(
        string id,
        [FromBody] AddItemsFromRecipeDto dto)
    {
        var command = new AddItemsFromRecipe.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Adds items from a collection to a ShoppingList with quantity merging.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/add-from-collection", Name = "AddItemsFromCollection")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> AddItemsFromCollection(
        string id,
        [FromBody] AddItemsFromCollectionDto dto)
    {
        var command = new AddItemsFromCollection.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
