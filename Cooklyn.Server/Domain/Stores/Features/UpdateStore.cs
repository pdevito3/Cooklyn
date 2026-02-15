namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdateStore
{
    public sealed record Command(string Id, StoreForUpdateDto Dto) : IRequest<StoreDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, StoreDto>
    {
        public async Task<StoreDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var store = await dbContext.Stores
                .Include(s => s.StoreAisles)
                .GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToStoreForUpdate();
            store.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return store.ToStoreDto();
        }
    }
}
