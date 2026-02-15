namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetItemCollection
{
    public sealed record Query(string Id) : IRequest<ItemCollectionDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, ItemCollectionDto>
    {
        public async Task<ItemCollectionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = await dbContext.ItemCollections
                .Include(ic => ic.Items)
                .GetById(request.Id, cancellationToken);

            return collection.ToItemCollectionDto();
        }
    }
}
