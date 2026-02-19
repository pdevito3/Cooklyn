namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;

public static class UpdateMealPlanEntry
{
    public sealed record Command(string Id, MealPlanEntryForUpdateDto Dto) : IRequest<MealPlanEntryDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, MealPlanEntryDto>
    {
        public async Task<MealPlanEntryDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await dbContext.MealPlanEntries.FindAsync([request.Id], cancellationToken)
                ?? throw new NotFoundException($"Meal plan entry {request.Id} not found.");

            var forUpdate = request.Dto.ToMealPlanEntryForUpdate();
            entry.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return entry.ToMealPlanEntryDto();
        }
    }
}
