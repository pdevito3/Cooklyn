namespace Cooklyn.Server.Domain.Ingredients.Features;

using Databases;
using MediatR;

public static class DeleteIngredient
{
    public sealed record Command(Guid Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var ingredient = await dbContext.Ingredients.GetById(request.Id, cancellationToken);

            dbContext.Ingredients.Remove(ingredient);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
