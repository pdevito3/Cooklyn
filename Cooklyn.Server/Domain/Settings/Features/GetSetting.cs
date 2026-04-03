namespace Cooklyn.Server.Domain.Settings.Features;

using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetSetting
{
    public sealed record Query(string Key) : IRequest<SettingDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, SettingDto>
    {
        public async Task<SettingDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var setting = await dbContext.Settings
                .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

            return new SettingDto
            {
                Key = request.Key,
                Value = setting?.Value
            };
        }
    }
}
