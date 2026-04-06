namespace Cooklyn.Server.Domain.Files.Controllers.v1;

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Services;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/files")]
public sealed class FilesController(IFileStorage fileStorage) : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    /// <summary>
    /// Proxies a file from S3-compatible storage, avoiding internal hostname issues
    /// when the storage service is not directly accessible from the browser.
    /// </summary>
    [HttpGet("{bucket}/{**key}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetFile(string bucket, string key, CancellationToken cancellationToken)
    {
        var stream = await fileStorage.GetFileAsync(bucket, key, cancellationToken);
        if (stream is null)
            return NotFound();

        if (!ContentTypeProvider.TryGetContentType(key, out var contentType))
            contentType = "application/octet-stream";

        return File(stream, contentType);
    }
}
