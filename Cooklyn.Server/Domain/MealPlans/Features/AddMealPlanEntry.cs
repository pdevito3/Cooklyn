namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddMealPlanEntry
{
    public sealed record Command(MealPlanEntryForCreationDto Dto) : IRequest<MealPlanEntryDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, MealPlanEntryDto>
    {
        public async Task<MealPlanEntryDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToMealPlanEntryForCreation();
            var entry = MealPlanEntry.Create(forCreation);

            await dbContext.MealPlanEntries.AddAsync(entry, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return entry.ToMealPlanEntryDto();
        }
    }
}
