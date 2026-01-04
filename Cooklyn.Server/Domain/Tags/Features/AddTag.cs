namespace Cooklyn.Server.Domain.Tags.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Services;

public static class AddTag
{
    public sealed record Command(TagForCreationDto Dto) : IRequest<TagDto>;

    public sealed class Handler(
        AppDbContext dbContext,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, TagDto>
    {
        public async Task<TagDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Tag), "Unable to determine tenant.");
            var forCreation = request.Dto.ToTagForCreation(tenantId);
            var tag = Tag.Create(forCreation);

            await dbContext.Tags.AddAsync(tag, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return tag.ToTagDto();
        }
    }
}
