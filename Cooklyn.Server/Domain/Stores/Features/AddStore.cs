namespace Cooklyn.Server.Domain.Stores.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Services;

public static class AddStore
{
    public sealed record Command(StoreForCreationDto Dto) : IRequest<StoreDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, StoreDto>
    {
        public async Task<StoreDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Store), "Unable to determine tenant.");
            var forCreation = request.Dto.ToStoreForCreation(tenantId);
            var store = Store.Create(forCreation);

            await dbContext.Stores.AddAsync(store, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return store.ToStoreDto();
        }
    }
}
