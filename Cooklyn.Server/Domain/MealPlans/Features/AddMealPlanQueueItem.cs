namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AddMealPlanQueueItem
{
    public sealed record Command(string QueueId, MealPlanQueueItemForCreationDto Dto) : IRequest<MealPlanQueueDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, MealPlanQueueDto>
    {
        public async Task<MealPlanQueueDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var queue = await dbContext.MealPlanQueues
                .Include(q => q.Items.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken)
                ?? throw new NotFoundException($"Meal plan queue {request.QueueId} not found.");

            var maxSortOrder = queue.Items.Any() ? queue.Items.Max(i => i.SortOrder) : -1;
            var forCreation = request.Dto.ToMealPlanQueueItemForCreation(request.QueueId, maxSortOrder + 1);
            var item = MealPlanQueueItem.Create(forCreation);

            queue.AddItem(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return queue.ToMealPlanQueueDto();
        }
    }
}
