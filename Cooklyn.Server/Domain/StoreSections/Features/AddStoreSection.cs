namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddStoreSection
{
    public sealed record Command(StoreSectionForCreationDto Dto) : IRequest<StoreSectionDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, StoreSectionDto>
    {
        public async Task<StoreSectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToStoreSectionForCreation();
            var storeSection = StoreSection.Create(forCreation);

            await dbContext.StoreSections.AddAsync(storeSection, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return storeSection.ToStoreSectionDto();
        }
    }
}
