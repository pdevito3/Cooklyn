namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using Exceptions;
using Importing.CopyMeThat;
using MediatR;
using Services;

public static class PreviewCmtImport
{
    public sealed record Command(IFormFile File) : IRequest<CmtImportPreviewDto>;

    public sealed class Handler(
        ICmtImportService cmtImportService,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, CmtImportPreviewDto>
    {
        public async Task<CmtImportPreviewDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Recipe), "Unable to determine tenant.");

            return await cmtImportService.ParseZipAsync(request.File, tenantId, cancellationToken);
        }
    }
}
