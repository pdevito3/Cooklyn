namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Services;

public static class AddItemCollection
{
    public sealed record Command(ItemCollectionForCreationDto Dto) : IRequest<ItemCollectionDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, ItemCollectionDto>
    {
        public async Task<ItemCollectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(ItemCollection), "Unable to determine tenant.");
            var forCreation = request.Dto.ToItemCollectionForCreation(tenantId);
            var collection = ItemCollection.Create(forCreation);

            await dbContext.ItemCollections.AddAsync(collection, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return collection.ToItemCollectionDto();
        }
    }
}
