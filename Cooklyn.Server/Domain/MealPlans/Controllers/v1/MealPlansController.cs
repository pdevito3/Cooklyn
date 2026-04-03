namespace Cooklyn.Server.Domain.MealPlans.Controllers.v1;

using Asp.Versioning;
using Domain.ShoppingLists.Dtos;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/meal-plans")]
[ApiVersion("1.0")]
public sealed class MealPlansController(IMediator mediator) : ControllerBase
{
    // Calendar

    [HttpGet("calendar", Name = "GetMealPlanCalendar")]
    [ProducesResponseType(typeof(IReadOnlyList<MealPlanDayDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MealPlanDayDto>>> GetMealPlanCalendar(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        var query = new GetMealPlanCalendar.Query(startDate, endDate);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("calendar-ingredients", Name = "GetMealPlanCalendarIngredients")]
    [ProducesResponseType(typeof(IReadOnlyList<MealPlanRecipeIngredientsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MealPlanRecipeIngredientsDto>>> GetMealPlanCalendarIngredients(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        var query = new GetMealPlanCalendarIngredients.Query(startDate, endDate);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    // Entries

    [HttpPost("entries", Name = "AddMealPlanEntry")]
    [ProducesResponseType(typeof(MealPlanEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MealPlanEntryDto>> AddMealPlanEntry(
        [FromBody] MealPlanEntryForCreationDto dto)
    {
        var command = new AddMealPlanEntry.Command(dto);
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("entries/{id}", Name = "UpdateMealPlanEntry")]
    [ProducesResponseType(typeof(MealPlanEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanEntryDto>> UpdateMealPlanEntry(
        string id,
        [FromBody] MealPlanEntryForUpdateDto dto)
    {
        var command = new UpdateMealPlanEntry.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("entries/{id}", Name = "DeleteMealPlanEntry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMealPlanEntry(string id)
    {
        var command = new DeleteMealPlanEntry.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("entries/{id}/move", Name = "MoveMealPlanEntry")]
    [ProducesResponseType(typeof(MealPlanEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanEntryDto>> MoveMealPlanEntry(
        string id,
        [FromBody] MoveMealPlanEntryDto dto)
    {
        var command = new MoveMealPlanEntry.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("entries/{id}/copy", Name = "CopyMealPlanEntry")]
    [ProducesResponseType(typeof(MealPlanEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanEntryDto>> CopyMealPlanEntry(
        string id,
        [FromBody] CopyMealPlanEntryDto dto)
    {
        var command = new CopyMealPlanEntry.Command(id, dto);
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    // Queues

    [HttpGet("queues", Name = "GetMealPlanQueues")]
    [ProducesResponseType(typeof(IReadOnlyList<MealPlanQueueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MealPlanQueueDto>>> GetMealPlanQueues()
    {
        var query = new GetMealPlanQueues.Query();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("queues", Name = "AddMealPlanQueue")]
    [ProducesResponseType(typeof(MealPlanQueueDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MealPlanQueueDto>> AddMealPlanQueue(
        [FromBody] MealPlanQueueForCreationDto dto)
    {
        var command = new AddMealPlanQueue.Command(dto);
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("queues/{id}", Name = "UpdateMealPlanQueue")]
    [ProducesResponseType(typeof(MealPlanQueueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanQueueDto>> UpdateMealPlanQueue(
        string id,
        [FromBody] MealPlanQueueForUpdateDto dto)
    {
        var command = new UpdateMealPlanQueue.Command(id, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("queues/{id}", Name = "DeleteMealPlanQueue")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMealPlanQueue(string id)
    {
        var command = new DeleteMealPlanQueue.Command(id);
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("queues/{queueId}/items", Name = "AddMealPlanQueueItem")]
    [ProducesResponseType(typeof(MealPlanQueueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanQueueDto>> AddMealPlanQueueItem(
        string queueId,
        [FromBody] MealPlanQueueItemForCreationDto dto)
    {
        var command = new AddMealPlanQueueItem.Command(queueId, dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("queues/{queueId}/items/{itemId}", Name = "DeleteMealPlanQueueItem")]
    [ProducesResponseType(typeof(MealPlanQueueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanQueueDto>> DeleteMealPlanQueueItem(
        string queueId,
        string itemId)
    {
        var command = new DeleteMealPlanQueueItem.Command(queueId, itemId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    // Add from queue to calendar

    [HttpPost("add-from-queue", Name = "AddToCalendarFromQueue")]
    [ProducesResponseType(typeof(MealPlanEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanEntryDto>> AddToCalendarFromQueue(
        [FromBody] AddToCalendarFromQueueDto dto)
    {
        var command = new AddToCalendarFromQueue.Command(dto);
        var result = await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    // Shopping list generation

    [HttpPost("generate-shopping-list", Name = "GenerateShoppingListFromMealPlan")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> GenerateShoppingList(
        [FromBody] BulkShoppingListFromMealPlanDto dto)
    {
        var command = new GenerateShoppingListFromMealPlan.Command(dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
