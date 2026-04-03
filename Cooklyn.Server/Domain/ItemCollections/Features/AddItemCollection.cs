namespace Cooklyn.Server.Domain.ItemCollections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddItemCollection
{
    public sealed record Command(ItemCollectionForCreationDto Dto) : IRequest<ItemCollectionDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, ItemCollectionDto>
    {
        public async Task<ItemCollectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToItemCollectionForCreation();
            var collection = ItemCollection.Create(forCreation);

            await dbContext.ItemCollections.AddAsync(collection, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return collection.ToItemCollectionDto();
        }
    }
}
