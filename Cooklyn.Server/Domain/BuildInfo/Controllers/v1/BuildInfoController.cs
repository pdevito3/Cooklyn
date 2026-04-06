namespace Cooklyn.Server.Domain.BuildInfo.Controllers.v1;

using System.Text.Json;
using Asp.Versioning;
using Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class BuildInfoController(IWebHostEnvironment env) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [HttpGet]
    public async Task<ActionResult<BuildInfoDto>> GetBuildInfo()
    {
        var path = Path.Combine(env.ContentRootPath, "build-info.json");

        if (!System.IO.File.Exists(path))
        {
            return Ok(new BuildInfoDto
            {
                CommitSha = "development",
                ShortSha = "dev",
                Branch = "local",
                BuildTimestamp = DateTime.UtcNow.ToString("o"),
                Commits = []
            });
        }

        var json = await System.IO.File.ReadAllTextAsync(path);
        var buildInfo = JsonSerializer.Deserialize<BuildInfoDto>(json, JsonOptions);
        return Ok(buildInfo);
    }
}
