namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;

public static class AddToCalendarFromQueue
{
    public sealed record Command(AddToCalendarFromQueueDto Dto) : IRequest<MealPlanEntryDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, MealPlanEntryDto>
    {
        public async Task<MealPlanEntryDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var queueItem = await dbContext.MealPlanQueueItems
                .FirstOrDefaultAsync(i => i.Id == request.Dto.QueueItemId, cancellationToken)
                ?? throw new NotFoundException($"Queue item {request.Dto.QueueItemId} not found.");

            var entryType = queueItem.RecipeId != null ? "Recipe" : "FreeText";

            var forCreation = new MealPlanEntryForCreation
            {
                Date = request.Dto.TargetDate,
                EntryType = entryType,
                RecipeId = queueItem.RecipeId,
                Title = queueItem.Title,
                Scale = queueItem.Scale,
                SortOrder = request.Dto.SortOrder
            };

            var entry = MealPlanEntry.Create(forCreation);
            await dbContext.MealPlanEntries.AddAsync(entry, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return entry.ToMealPlanEntryDto();
        }
    }
}
