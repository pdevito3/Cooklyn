namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetStore
{
    public sealed record Query(string Id) : IRequest<StoreDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, StoreDto>
    {
        public async Task<StoreDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreAisles)
                .Include(s => s.StoreDefaultCollections)
                    .ThenInclude(sdc => sdc.ItemCollection)
                .GetById(request.Id, cancellationToken);

            return store.ToStoreDto();
        }
    }
}
