namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class UpdateTag
{
    public sealed record Command(string Id, TagForUpdateDto Dto) : IRequest<TagDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, TagDto>
    {
        public async Task<TagDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tag = await dbContext.Tags.GetById(request.Id, cancellationToken);

            var forUpdate = request.Dto.ToTagForUpdate();
            tag.Update(forUpdate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return tag.ToTagDto();
        }
    }
}
