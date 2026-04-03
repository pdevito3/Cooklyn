namespace Cooklyn.Server.Domain.Settings.Controllers.v1;

using Asp.Versioning;
using Dtos;
using Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class SettingsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get a setting by key
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<SettingDto>> GetSetting(string key)
    {
        var result = await mediator.Send(new GetSetting.Query(key));
        return Ok(result);
    }

    /// <summary>
    /// Create or update a setting by key
    /// </summary>
    [HttpPut("{key}")]
    public async Task<ActionResult<SettingDto>> UpsertSetting(string key, UpsertSettingDto dto)
    {
        var result = await mediator.Send(new UpsertSetting.Command(key, dto));
        return Ok(result);
    }
}
