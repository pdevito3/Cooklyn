namespace Cooklyn.Server.Domain.StoreSections.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Services;

public static class AddStoreSection
{
    public sealed record Command(StoreSectionForCreationDto Dto) : IRequest<StoreSectionDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, StoreSectionDto>
    {
        public async Task<StoreSectionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(StoreSection), "Unable to determine tenant.");
            var forCreation = request.Dto.ToStoreSectionForCreation(tenantId);
            var storeSection = StoreSection.Create(forCreation);

            await dbContext.StoreSections.AddAsync(storeSection, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return storeSection.ToStoreSectionDto();
        }
    }
}
