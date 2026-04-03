namespace Cooklyn.Server.Domain.Settings.Features;

using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpsertSetting
{
    public sealed record Command(string Key, UpsertSettingDto Dto) : IRequest<SettingDto>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Command, SettingDto>
    {
        public async Task<SettingDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var setting = await dbContext.Settings
                .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

            if (setting is null)
            {
                setting = Setting.Create(request.Key, request.Dto.Value);
                await dbContext.Settings.AddAsync(setting, cancellationToken);
            }
            else
            {
                setting.UpdateValue(request.Dto.Value);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return new SettingDto
            {
                Key = setting.Key,
                Value = setting.Value
            };
        }
    }
}
