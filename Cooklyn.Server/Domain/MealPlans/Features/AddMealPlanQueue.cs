namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddMealPlanQueue
{
    public sealed record Command(MealPlanQueueForCreationDto Dto) : IRequest<MealPlanQueueDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, MealPlanQueueDto>
    {
        public async Task<MealPlanQueueDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToMealPlanQueueForCreation();
            var queue = MealPlanQueue.Create(forCreation);

            await dbContext.MealPlanQueues.AddAsync(queue, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return queue.ToMealPlanQueueDto();
        }
    }
}
