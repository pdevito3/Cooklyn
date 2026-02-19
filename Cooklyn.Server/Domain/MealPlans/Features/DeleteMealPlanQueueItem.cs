namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class DeleteMealPlanQueueItem
{
    public sealed record Command(string QueueId, string ItemId) : IRequest<MealPlanQueueDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, MealPlanQueueDto>
    {
        public async Task<MealPlanQueueDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var queue = await dbContext.MealPlanQueues
                .Include(q => q.Items.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken)
                ?? throw new NotFoundException($"Meal plan queue {request.QueueId} not found.");

            var item = queue.Items.FirstOrDefault(i => i.Id == request.ItemId)
                ?? throw new NotFoundException($"Queue item {request.ItemId} not found.");

            queue.RemoveItem(item);
            dbContext.MealPlanQueueItems.Remove(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return queue.ToMealPlanQueueDto();
        }
    }
}
