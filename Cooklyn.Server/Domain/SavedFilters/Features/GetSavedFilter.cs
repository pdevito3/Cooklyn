namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class GetSavedFilter
{
    public sealed record Query(string Id) : IRequest<SavedFilterDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, SavedFilterDto>
    {
        public async Task<SavedFilterDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var savedFilter = await dbContext.SavedFilters.GetById(request.Id, cancellationToken);
            return savedFilter.ToSavedFilterDto();
        }
    }
}
