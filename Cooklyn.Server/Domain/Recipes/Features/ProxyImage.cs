namespace Cooklyn.Server.Domain.Recipes.Features;

using Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public static class ProxyImage
{
    public sealed record Query(string Url) : IRequest<FileStreamResult>;

    public sealed class Handler(IHttpClientFactory httpClientFactory)
        : IRequestHandler<Query, FileStreamResult>
    {
        private const long MaxResponseBytes = 10 * 1024 * 1024; // 10 MB

        private static readonly HashSet<string> AllowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp",
            "image/avif",
        ];

        public async Task<FileStreamResult> Handle(Query request, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                throw new ValidationException("Url", "Please provide a valid HTTP or HTTPS URL.");
            }

            var client = httpClientFactory.CreateClient("RecipeImport");

            using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            if (!AllowedContentTypes.Contains(contentType))
            {
                throw new ValidationException("Url", "The URL does not point to a supported image format.");
            }

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength > MaxResponseBytes)
            {
                throw new ValidationException("Url", "The image is too large (max 10 MB).");
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Copy to a memory stream so we own the data and can dispose the HTTP response
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);

            if (memoryStream.Length > MaxResponseBytes)
            {
                memoryStream.Dispose();
                throw new ValidationException("Url", "The image is too large (max 10 MB).");
            }

            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, contentType);
        }
    }
}
