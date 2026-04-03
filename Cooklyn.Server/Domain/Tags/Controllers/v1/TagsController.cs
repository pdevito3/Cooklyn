namespace Cooklyn.Server.Domain.Tags.Controllers.v1;

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
public sealed class TagsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a single Tag by ID.
    /// </summary>
    [HttpGet("{id}", Name = "GetTag")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        var query = new GetTag.Query(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a paginated list of Tags.
    /// </summary>
    [HttpGet(Name = "GetTagList")]
    [ProducesResponseType(typeof(PagedList<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<TagDto>>> GetTagList(
        [FromQuery] TagParametersDto parameters)
    {
        var query = new GetTagList.Query(parameters);
        var result = await mediator.Send(query);

        Response.AddPaginationHeader(result);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new Tag.
    /// </summary>
    [HttpPost(Name = "AddTag")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagDto>> AddTag(
        [FromBody] TagForCreationDto dto)
    {
        var command = new AddTag.Command(dto);
        var result = await mediator.Send(command);

        return CreatedAtRoute("GetTag",
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing Tag.
    /// </summary>
    [HttpPut("{id}", Name = "UpdateTag")]
    [ProducesResponseType(typeof(TagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TagDto>> UpdateTag(
        string id,
        [FromBody] TagForUpdateDto dto)
    {
        var command = new UpdateTag.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes a Tag.
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteTag")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTag(string id)
    {
        var command = new DeleteTag.Command(id);
        await mediator.Send(command);
        return NoContent();
    }
}
