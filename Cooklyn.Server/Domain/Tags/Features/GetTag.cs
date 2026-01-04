namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class GetTag
{
    public sealed record Query(Guid Id) : IRequest<TagDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, TagDto>
    {
        public async Task<TagDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var tag = await dbContext.Tags.GetById(request.Id, cancellationToken);
            return tag.ToTagDto();
        }
    }
}
