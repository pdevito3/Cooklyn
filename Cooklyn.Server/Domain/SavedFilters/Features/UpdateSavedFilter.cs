namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class UpdateSavedFilter
{
    public sealed record Command(string Id, SavedFilterForUpdateDto Dto) : IRequest<SavedFilterDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, SavedFilterDto>
    {
        public async Task<SavedFilterDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var savedFilter = await dbContext.SavedFilters.GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToSavedFilterForUpdate();
            savedFilter.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return savedFilter.ToSavedFilterDto();
        }
    }
}
