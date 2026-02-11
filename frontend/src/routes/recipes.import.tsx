import { ArrowLeft02Icon, Loading03Icon } from "@hugeicons/core-free-icons";
import { HugeiconsIcon } from "@hugeicons/react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";

import { RecipeForm, type RecipeFormValues } from "@/components/recipe-form";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  useCreateRecipe,
  useImportRecipePreview,
  type ImportRecipePreviewDto,
} from "@/domain/recipes";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/recipes/import")({
  component: ImportRecipePage,
});

function ImportRecipePage() {
  const navigate = useNavigate();
  const [url, setUrl] = useState("");
  const [preview, setPreview] = useState<ImportRecipePreviewDto | null>(null);
  const [selectedImageUrl, setSelectedImageUrl] = useState<string | null>(null);

  const importPreview = useImportRecipePreview();
  const createRecipe = useCreateRecipe();

  const isCreating = createRecipe.isPending;

  const handleImport = () => {
    if (!url.trim()) return;
    importPreview.mutate(url.trim(), {
      onSuccess: (data) => {
        setPreview(data);
        // Auto-select the first image if available
        if (data.images.length > 0) {
          setSelectedImageUrl(data.images[0].url);
        }
      },
    });
  };

  const handleSubmit = (values: RecipeFormValues) => {
    createRecipe.mutate(
      {
        title: values.title,
        description: values.description,
        imageS3Bucket: null,
        imageS3Key: null,
        rating: values.rating,
        source: values.source,
        servings: values.servings,
        steps: values.steps,
        notes: values.notes,
        tagIds: [],
        flags: values.flags.map((f) => f.value),
        ingredients: values.ingredients,
        nutritionInfo: null,
        imageUrl: selectedImageUrl,
      },
      {
        onSuccess: (recipe) => {
          navigate({ to: "/recipes/$id", params: { id: recipe.id } });
        },
      },
    );
  };

  const handleBack = () => {
    navigate({ to: "/recipes" });
  };

  const defaultValues: Partial<RecipeFormValues> | undefined = preview
    ? {
        title: preview.title ?? "",
        description: preview.description ?? null,
        source: preview.source ?? null,
        servings: preview.servings ?? null,
        steps: preview.steps ?? null,
        flags: [],
        ingredients: preview.ingredients,
      }
    : undefined;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={handleBack}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-5 w-5" />
        </Button>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Import Recipe</h1>
          <p className="text-muted-foreground">
            Import a recipe from a website URL
          </p>
        </div>
      </div>

      <div className="mx-auto max-w-3xl space-y-6">
        {/* URL Input */}
        <Card>
          <CardHeader>
            <CardTitle>Recipe URL</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-3">
              <div className="flex-1">
                <Label htmlFor="import-url" className="sr-only">
                  Recipe URL
                </Label>
                <Input
                  id="import-url"
                  type="url"
                  autoFocus
                  placeholder="https://www.example.com/recipe/..."
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      handleImport();
                    }
                  }}
                  disabled={importPreview.isPending}
                />
              </div>
              <Button
                onClick={handleImport}
                disabled={!url.trim() || importPreview.isPending}
              >
                {importPreview.isPending ? (
                  <>
                    <HugeiconsIcon
                      icon={Loading03Icon}
                      className="mr-2 h-4 w-4 animate-spin"
                    />
                    Importing...
                  </>
                ) : (
                  "Import"
                )}
              </Button>
            </div>

            {importPreview.isError && (
              <p className="mt-3 text-sm font-medium text-destructive">
                {importPreview.error instanceof Error
                  ? importPreview.error.message
                  : "Failed to import recipe. The site may not include structured recipe data."}
              </p>
            )}
          </CardContent>
        </Card>

        {/* Image Picker */}
        {preview && preview.images.length > 0 && (
          <Card>
            <CardHeader>
              <CardTitle>Select an Image</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
                {preview.images.slice(0, 12).map((image) => (
                  <button
                    key={image.url}
                    type="button"
                    onClick={() =>
                      setSelectedImageUrl(
                        selectedImageUrl === image.url ? null : image.url,
                      )
                    }
                    className={cn(
                      "relative aspect-video overflow-hidden rounded-lg border-2 transition-all",
                      selectedImageUrl === image.url
                        ? "border-primary ring-2 ring-primary/20"
                        : "border-border hover:border-primary/50",
                    )}
                  >
                    <img
                      src={image.url}
                      alt={image.alt ?? "Recipe image"}
                      className="h-full w-full object-cover"
                      loading="lazy"
                      onError={(e) => {
                        (e.target as HTMLImageElement).style.display = "none";
                      }}
                    />
                    {selectedImageUrl === image.url && (
                      <div className="absolute inset-0 flex items-center justify-center bg-primary/10">
                        <div className="rounded-full bg-primary px-2 py-1 text-xs font-medium text-primary-foreground">
                          Selected
                        </div>
                      </div>
                    )}
                  </button>
                ))}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Recipe Form (pre-filled) */}
        {preview && (
          <>
            <RecipeForm
              key={preview.source}
              defaultValues={defaultValues}
              onSubmit={handleSubmit}
              onCancel={handleBack}
              isSubmitting={isCreating}
              submitLabel="Create Recipe"
            />

            {createRecipe.isError && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
                <p className="font-medium">Error creating recipe</p>
                <p className="mt-1 text-sm">
                  {createRecipe.error instanceof Error
                    ? createRecipe.error.message
                    : "An unexpected error occurred"}
                </p>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
