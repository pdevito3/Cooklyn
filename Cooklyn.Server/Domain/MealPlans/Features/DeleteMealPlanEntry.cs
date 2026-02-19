namespace Cooklyn.Server.Domain.MealPlans.Features;

using Databases;
using Exceptions;
using MediatR;

public static class DeleteMealPlanEntry
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var entry = await dbContext.MealPlanEntries.FindAsync([request.Id], cancellationToken)
                ?? throw new NotFoundException($"Meal plan entry {request.Id} not found.");

            dbContext.MealPlanEntries.Remove(entry);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
