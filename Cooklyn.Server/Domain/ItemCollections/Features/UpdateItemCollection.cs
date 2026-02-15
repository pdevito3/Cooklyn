namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateItemCollection
{
    public sealed record Command(string Id, ItemCollectionForUpdateDto Dto) : IRequest<ItemCollectionDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, ItemCollectionDto>
    {
        public async Task<ItemCollectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var collection = await dbContext.ItemCollections
                .Include(ic => ic.Items)
                .GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToItemCollectionForUpdate();
            collection.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return collection.ToItemCollectionDto();
        }
    }
}
