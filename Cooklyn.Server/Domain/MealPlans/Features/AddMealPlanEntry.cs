namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Services;

public static class AddMealPlanEntry
{
    public sealed record Command(MealPlanEntryForCreationDto Dto) : IRequest<MealPlanEntryDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, MealPlanEntryDto>
    {
        public async Task<MealPlanEntryDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new Exceptions.ValidationException(nameof(MealPlanEntry), "Unable to determine tenant.");

            var forCreation = request.Dto.ToMealPlanEntryForCreation(tenantId);
            var entry = MealPlanEntry.Create(forCreation);

            await dbContext.MealPlanEntries.AddAsync(entry, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return entry.ToMealPlanEntryDto();
        }
    }
}
