namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Services;

public static class AddSavedFilter
{
    public sealed record Command(SavedFilterForCreationDto Dto) : IRequest<SavedFilterDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, SavedFilterDto>
    {
        public async Task<SavedFilterDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(SavedFilter), "Unable to determine tenant.");
            var forCreation = request.Dto.ToSavedFilterForCreation(tenantId);
            var savedFilter = SavedFilter.Create(forCreation);

            await dbContext.SavedFilters.AddAsync(savedFilter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return savedFilter.ToSavedFilterDto();
        }
    }
}
