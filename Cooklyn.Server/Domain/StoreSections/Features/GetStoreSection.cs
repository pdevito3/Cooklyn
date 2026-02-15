namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class GetStoreSection
{
    public sealed record Query(string Id) : IRequest<StoreSectionDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, StoreSectionDto>
    {
        public async Task<StoreSectionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var storeSection = await dbContext.StoreSections.GetById(request.Id, cancellationToken);
            return storeSection.ToStoreSectionDto();
        }
    }
}
