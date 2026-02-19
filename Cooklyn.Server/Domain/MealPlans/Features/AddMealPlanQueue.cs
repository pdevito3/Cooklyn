namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Services;

public static class AddMealPlanQueue
{
    public sealed record Command(MealPlanQueueForCreationDto Dto) : IRequest<MealPlanQueueDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, MealPlanQueueDto>
    {
        public async Task<MealPlanQueueDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new Exceptions.ValidationException(nameof(MealPlanQueue), "Unable to determine tenant.");

            var forCreation = request.Dto.ToMealPlanQueueForCreation(tenantId);
            var queue = MealPlanQueue.Create(forCreation);

            await dbContext.MealPlanQueues.AddAsync(queue, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return queue.ToMealPlanQueueDto();
        }
    }
}
