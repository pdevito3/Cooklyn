namespace Cooklyn.Server.Domain.SavedFilters.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Extensions;

[ApiController]
[Route("api/v{v:apiVersion}/saved-filters")]
[ApiVersion("1.0")]
public sealed class SavedFiltersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single SavedFilter by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id}", Name = "GetSavedFilter")]
    [ProducesResponseType(typeof(SavedFilterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SavedFilterDto>> GetSavedFilter(string id)
    {
        var query = new GetSavedFilter.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of SavedFilters filtered by context.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetSavedFilterList")]
    [ProducesResponseType(typeof(PagedList<SavedFilterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<SavedFilterDto>>> GetSavedFilterList(
        [FromQuery] SavedFilterParametersDto parameters)
    {
        var query = new GetSavedFilterList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new SavedFilter.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddSavedFilter")]
    [ProducesResponseType(typeof(SavedFilterDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SavedFilterDto>> AddSavedFilter(
        [FromBody] SavedFilterForCreationDto dto)
    {
        var command = new AddSavedFilter.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetSavedFilter",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing SavedFilter.
    /// </summary>
    [Authorize]
    [HttpPut("{id}", Name = "UpdateSavedFilter")]
    [ProducesResponseType(typeof(SavedFilterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SavedFilterDto>> UpdateSavedFilter(
        string id,
        [FromBody] SavedFilterForUpdateDto dto)
    {
        var command = new UpdateSavedFilter.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a SavedFilter.
    /// </summary>
    [Authorize]
    [HttpDelete("{id}", Name = "DeleteSavedFilter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteSavedFilter(string id)
    {
        var command = new DeleteSavedFilter.Command(id);
        await mediator.Send(command);
        return NoContent();
    }
}
