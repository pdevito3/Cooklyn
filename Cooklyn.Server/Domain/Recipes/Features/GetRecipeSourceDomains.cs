namespace Cooklyn.Server.Domain.Recipes.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetRecipeSourceDomains
{
    public sealed record Query : IRequest<IReadOnlyList<string>>;

    public sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, IReadOnlyList<string>>
    {
        public async Task<IReadOnlyList<string>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sources = await dbContext.Recipes
                .AsNoTracking()
                .Where(r => r.Source != null && r.Source != "")
                .Select(r => r.Source!)
                .Distinct()
                .ToListAsync(cancellationToken);

            return sources
                .Select(TryGetDomain)
                .Where(d => d is not null)
                .Select(d => d!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string? TryGetDomain(string source)
        {
            if (!Uri.TryCreate(source, UriKind.Absolute, out var uri))
                return null;
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return null;

            var host = uri.Host;
            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                host = host[4..];
            return string.IsNullOrWhiteSpace(host) ? null : host.ToLowerInvariant();
        }
    }
}
