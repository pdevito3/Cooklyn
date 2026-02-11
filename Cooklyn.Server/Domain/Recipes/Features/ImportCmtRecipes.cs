namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using Exceptions;
using Importing.CopyMeThat;
using MediatR;
using Services;

public static class ImportCmtRecipes
{
    public sealed record Command(IFormFile File, CmtImportRequestDto Request) : IRequest<CmtImportResultDto>;

    public sealed class Handler(
        ICmtImportService cmtImportService,
        ITenantIdProvider tenantIdProvider,
        ICurrentUserService currentUserService) : IRequestHandler<Command, CmtImportResultDto>
    {
        public async Task<CmtImportResultDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var tenantId = await tenantIdProvider.GetTenantIdAsync(currentUserService.UserIdentifier!)
                ?? throw new ValidationException(nameof(Recipe), "Unable to determine tenant.");

            return await cmtImportService.ImportRecipesAsync(
                request.File,
                request.Request,
                tenantId,
                cancellationToken);
        }
    }
}
