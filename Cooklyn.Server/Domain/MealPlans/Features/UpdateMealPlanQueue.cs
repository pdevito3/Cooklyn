namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateMealPlanQueue
{
    public sealed record Command(string Id, MealPlanQueueForUpdateDto Dto) : IRequest<MealPlanQueueDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, MealPlanQueueDto>
    {
        public async Task<MealPlanQueueDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var queue = await dbContext.MealPlanQueues
                .Include(q => q.Items.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException($"Meal plan queue {request.Id} not found.");

            var forUpdate = request.Dto.ToMealPlanQueueForUpdate();
            queue.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return queue.ToMealPlanQueueDto();
        }
    }
}
