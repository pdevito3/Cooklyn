namespace Cooklyn.Server.Domain.Stores.Controllers.v1;

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
public sealed class StoresController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single Store by ID.
    /// </summary>
    [HttpGet("{id}", Name = "GetStore")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreDto>> GetStore(string id)
    {
        var query = new GetStore.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of Stores.
    /// </summary>
    [HttpGet(Name = "GetStoreList")]
    [ProducesResponseType(typeof(PagedList<StoreDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<StoreDto>>> GetStoreList(
        [FromQuery] StoreParametersDto parameters)
    {
        var query = new GetStoreList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new Store.
    /// </summary>
    [HttpPost(Name = "AddStore")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreDto>> AddStore(
        [FromBody] StoreForCreationDto dto)
    {
        var command = new AddStore.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetStore",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing Store.
    /// </summary>
    [HttpPut("{id}", Name = "UpdateStore")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreDto>> UpdateStore(
        string id,
        [FromBody] StoreForUpdateDto dto)
    {
        var command = new UpdateStore.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a Store.
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteStore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteStore(string id)
    {
        var command = new DeleteStore.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Updates the aisle ordering for a Store.
    /// </summary>
    [HttpPut("{id}/aisles", Name = "UpdateStoreAisles")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreDto>> UpdateStoreAisles(
        string id,
        [FromBody] List<StoreAisleForUpdateDto> aisles)
    {
        var command = new UpdateStoreAisles.Command(id, aisles);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates the default collections for a Store.
    /// </summary>
    [HttpPut("{id}/default-collections", Name = "UpdateStoreDefaultCollections")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreDto>> UpdateStoreDefaultCollections(
        string id,
        [FromBody] UpdateStoreDefaultCollectionsDto dto)
    {
        var command = new UpdateStoreDefaultCollections.Command(id, dto.ItemCollectionIds);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
