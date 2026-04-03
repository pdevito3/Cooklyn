namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using Importing.CopyMeThat;
using MediatR;

public static class PreviewCmtImport
{
    public sealed record Command(IFormFile File) : IRequest<CmtImportPreviewDto>;

    public sealed class Handler(
        ICmtImportService cmtImportService) : IRequestHandler<Command, CmtImportPreviewDto>
    {
        public async Task<CmtImportPreviewDto> Handle(Command request, CancellationToken cancellationToken)
        {
            return await cmtImportService.ParseZipAsync(request.File, cancellationToken);
        }
    }
}
