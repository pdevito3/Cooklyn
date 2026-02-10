namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using MediatR;

public static class DeleteTag
{
    public sealed record Command(string Id) : IRequest;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var tag = await dbContext.Tags.GetById(request.Id, cancellationToken);

            dbContext.Tags.Remove(tag);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
