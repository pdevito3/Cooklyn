namespace Cooklyn.Server.Domain.Users.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;

public static class UpdateUserDefaultStore
{
    public sealed record Command(string Id, string? StoreId) : IRequest<UserDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, UserDto>
    {
        public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.GetById(request.Id, cancellationToken);

            user.UpdateDefaultStore(request.StoreId);

            await dbContext.SaveChangesAsync(cancellationToken);

            return user.ToUserDto();
        }
    }
}
