namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class AddTag
{
    public sealed record Command(TagForCreationDto Dto) : IRequest<TagDto>;

    public sealed class Handler(
        AppDbContext dbContext) : IRequestHandler<Command, TagDto>
    {
        public async Task<TagDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var forCreation = request.Dto.ToTagForCreation();
            var tag = Tag.Create(forCreation);

            await dbContext.Tags.AddAsync(tag, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return tag.ToTagDto();
        }
    }
}
