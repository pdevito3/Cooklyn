namespace Cooklyn.Server.Domain.ItemCollections.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Extensions;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class ItemCollectionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single ItemCollection by ID with all items.
    /// </summary>
    [HttpGet("{id}", Name = "GetItemCollection")]
    [ProducesResponseType(typeof(ItemCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemCollectionDto>> GetItemCollection(string id)
    {
        var query = new GetItemCollection.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of ItemCollections.
    /// </summary>
    [HttpGet(Name = "GetItemCollectionList")]
    [ProducesResponseType(typeof(PagedList<ItemCollectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<ItemCollectionDto>>> GetItemCollectionList(
        [FromQuery] ItemCollectionParametersDto parameters)
    {
        var query = new GetItemCollectionList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new ItemCollection.
    /// </summary>
    [HttpPost(Name = "AddItemCollection")]
    [ProducesResponseType(typeof(ItemCollectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemCollectionDto>> AddItemCollection(
        [FromBody] ItemCollectionForCreationDto dto)
    {
        var command = new AddItemCollection.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetItemCollection",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing ItemCollection.
    /// </summary>
    [HttpPut("{id}", Name = "UpdateItemCollection")]
    [ProducesResponseType(typeof(ItemCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemCollectionDto>> UpdateItemCollection(
        string id,
        [FromBody] ItemCollectionForUpdateDto dto)
    {
        var command = new UpdateItemCollection.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an ItemCollection.
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteItemCollection")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteItemCollection(string id)
    {
        var command = new DeleteItemCollection.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Replaces all items in an ItemCollection.
    /// </summary>
    [HttpPut("{id}/items", Name = "UpdateItemCollectionItems")]
    [ProducesResponseType(typeof(ItemCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemCollectionDto>> UpdateItemCollectionItems(
        string id,
        [FromBody] List<ItemCollectionItemForCreationDto> items)
    {
        var command = new UpdateItemCollectionItems.Command(id, items);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
