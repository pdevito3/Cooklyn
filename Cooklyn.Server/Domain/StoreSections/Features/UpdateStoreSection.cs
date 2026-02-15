namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class UpdateStoreSection
{
    public sealed record Command(string Id, StoreSectionForUpdateDto Dto) : IRequest<StoreSectionDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, StoreSectionDto>
    {
        public async Task<StoreSectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var storeSection = await dbContext.StoreSections.GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToStoreSectionForUpdate();
            storeSection.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return storeSection.ToStoreSectionDto();
        }
    }
}
