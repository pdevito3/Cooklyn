namespace Cooklyn.Server.Domain.Recipes.Features;

using Dtos;
using MediatR;
using Importing;

public static class ImportRecipePreview
{
    public sealed record Command(ImportRecipePreviewRequestDto Dto) : IRequest<ImportRecipePreviewDto>;

    public sealed class Handler(IRecipeImportService recipeImportService)
        : IRequestHandler<Command, ImportRecipePreviewDto>
    {
        public async Task<ImportRecipePreviewDto> Handle(Command request, CancellationToken cancellationToken)
        {
            return await recipeImportService.ImportFromUrlAsync(request.Dto.Url, cancellationToken);
        }
    }
}
