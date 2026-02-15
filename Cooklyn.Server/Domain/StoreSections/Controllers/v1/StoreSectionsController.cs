namespace Cooklyn.Server.Domain.StoreSections.Controllers.v1;

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
public sealed class StoreSectionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single StoreSection by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id}", Name = "GetStoreSection")]
    [ProducesResponseType(typeof(StoreSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreSectionDto>> GetStoreSection(string id)
    {
        var query = new GetStoreSection.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of StoreSections.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetStoreSectionList")]
    [ProducesResponseType(typeof(PagedList<StoreSectionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<StoreSectionDto>>> GetStoreSectionList(
        [FromQuery] StoreSectionParametersDto parameters)
    {
        var query = new GetStoreSectionList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new StoreSection.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddStoreSection")]
    [ProducesResponseType(typeof(StoreSectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreSectionDto>> AddStoreSection(
        [FromBody] StoreSectionForCreationDto dto)
    {
        var command = new AddStoreSection.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetStoreSection",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing StoreSection.
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = "UpdateStoreSection")]
    [ProducesResponseType(typeof(StoreSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoreSectionDto>> UpdateStoreSection(
        string id,
        [FromBody] StoreSectionForUpdateDto dto)
    {
        var command = new UpdateStoreSection.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a StoreSection.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}", Name = "DeleteStoreSection")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteStoreSection(string id)
    {
        var command = new DeleteStoreSection.Command(id);
        await mediator.Send(command);
        return NoContent();
    }
}
