namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using Importing.CopyMeThat;
using MediatR;

public static class ImportCmtRecipes
{
    public sealed record Command(IFormFile File, CmtImportRequestDto Request) : IRequest<CmtImportResultDto>;

    public sealed class Handler(
        ICmtImportService cmtImportService) : IRequestHandler<Command, CmtImportResultDto>
    {
        public async Task<CmtImportResultDto> Handle(Command request, CancellationToken cancellationToken)
        {
            return await cmtImportService.ImportRecipesAsync(
                request.File,
                request.Request,
                cancellationToken);
        }
    }
}
