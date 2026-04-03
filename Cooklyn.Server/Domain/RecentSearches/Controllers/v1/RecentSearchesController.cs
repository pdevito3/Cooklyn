namespace Cooklyn.Server.Domain.RecentSearches.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/recent-searches")]
[ApiVersion("1.0")]
public sealed class RecentSearchesController(IMediator mediator) : ControllerBase
{
    /// <summary>Gets the most recent searches.</summary>
    [HttpGet(Name = "GetRecentSearchList")]
    [ProducesResponseType(typeof(List<RecentSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RecentSearchDto>>> GetRecentSearchList(
        [FromQuery] RecentSearchParametersDto parameters)
    {
        var query = new GetRecentSearchList.Query(parameters);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>Creates a new recent search entry.</summary>
    [HttpPost(Name = "AddRecentSearch")]
    [ProducesResponseType(typeof(RecentSearchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecentSearchDto>> AddRecentSearch(
        [FromBody] RecentSearchForCreationDto dto)
    {
        var command = new AddRecentSearch.Command(dto);
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Deletes a single recent search entry.</summary>
    [HttpDelete("{id}", Name = "DeleteRecentSearch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRecentSearch(string id)
    {
        var command = new DeleteRecentSearch.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>Clears all recent searches for the current tenant.</summary>
    [HttpDelete(Name = "ClearRecentSearches")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ClearRecentSearches()
    {
        var command = new ClearRecentSearches.Command();
        await mediator.Send(command);
        return NoContent();
    }
}
