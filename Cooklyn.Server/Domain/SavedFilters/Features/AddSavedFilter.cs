namespace Cooklyn.Server.Domain.SavedFilters.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddSavedFilter
{
    public sealed record Command(SavedFilterForCreationDto Dto) : IRequest<SavedFilterDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, SavedFilterDto>
    {
        public async Task<SavedFilterDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToSavedFilterForCreation();
            var savedFilter = SavedFilter.Create(forCreation);

            await dbContext.SavedFilters.AddAsync(savedFilter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return savedFilter.ToSavedFilterDto();
        }
    }
}
