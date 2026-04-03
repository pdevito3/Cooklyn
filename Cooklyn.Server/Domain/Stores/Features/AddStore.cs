namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddStore
{
    public sealed record Command(StoreForCreationDto Dto) : IRequest<StoreDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, StoreDto>
    {
        public async Task<StoreDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToStoreForCreation();
            var store = Store.Create(forCreation);

            await dbContext.Stores.AddAsync(store, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return store.ToStoreDto();
        }
    }
}
