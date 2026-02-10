import {
  ArrowLeft02Icon,
  Delete01Icon,
  Edit01Icon,
  FavouriteIcon,
  Image02Icon,
} from "@hugeicons/core-free-icons";
import { HugeiconsIcon } from "@hugeicons/react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useState } from "react";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { SourceImagePicker } from "@/components/source-image-picker";
import { ScaleInput, formatScaledAmount } from "@/components/scale-input";
import { StepViewer } from "@/components/step-viewer";
import {
  useDeleteRecipe,
  useRecipe,
  useToggleRecipeFavorite,
  formatUnit,
} from "@/domain/recipes";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/recipes/$id/")({
  component: RecipeDetailPage,
});

function RecipeDetailPage() {
  const { id } = Route.useParams();
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [imagePickerOpen, setImagePickerOpen] = useState(false);
  const [scale, setScale] = useState(1);

  const { data: recipe, isLoading, error } = useRecipe(id);
  const deleteRecipe = useDeleteRecipe();
  const toggleFavorite = useToggleRecipeFavorite();

  const handleBack = () => {
    navigate({ to: "/recipes" });
  };

  const handleEdit = () => {
    navigate({ to: "/recipes/$id/edit", params: { id } });
  };

  const handleDelete = () => {
    setDeleteDialogOpen(true);
  };

  const confirmDelete = () => {
    deleteRecipe.mutate(id, {
      onSuccess: () => {
        navigate({ to: "/recipes" });
      },
    });
  };

  const handleToggleFavorite = () => {
    toggleFavorite.mutate(id);
  };

  const placeholderImage =
    'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="800" height="400" viewBox="0 0 800 400"%3E%3Crect fill="%23e2e8f0" width="800" height="400"/%3E%3Ctext fill="%2394a3b8" font-family="system-ui" font-size="24" x="50%25" y="50%25" text-anchor="middle" dominant-baseline="middle"%3ENo Image%3C/text%3E%3C/svg%3E';

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Skeleton className="w-10 h-10" />
          <Skeleton className="w-64 h-8" />
        </div>
        <Skeleton className="aspect-[4/3] w-full rounded-lg" />
        <div className="space-y-4">
          <Skeleton className="w-48 h-6" />
          <Skeleton className="w-full h-4" />
          <Skeleton className="w-3/4 h-4" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={handleBack}>
            <HugeiconsIcon icon={ArrowLeft02Icon} className="w-5 h-5" />
          </Button>
          <h1 className="text-3xl font-bold tracking-tight">
            Recipe Not Found
          </h1>
        </div>
        <div className="p-4 border rounded-lg border-destructive/20 bg-destructive/10 text-destructive">
          <p className="font-medium">Error loading recipe</p>
          <p className="mt-1 text-sm">
            {error instanceof Error ? error.message : "Failed to load recipe"}
          </p>
        </div>
        <Button onClick={handleBack}>Back to Recipes</Button>
      </div>
    );
  }

  if (!recipe) {
    return null;
  }

  return (
    <div className="max-w-5xl space-y-6">
      {/* Header */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={handleBack}>
            <HugeiconsIcon icon={ArrowLeft02Icon} className="w-5 h-5" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">
              {recipe.title}
            </h1>
            {recipe.source && (
              <p className="text-muted-foreground">
                Source:{" "}
                {recipe.source.startsWith("http") ? (
                  <a
                    href={recipe.source}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="underline hover:text-foreground"
                  >
                    {recipe.source}
                  </a>
                ) : (
                  recipe.source
                )}
              </p>
            )}
          </div>
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="icon"
            onClick={handleToggleFavorite}
            disabled={toggleFavorite.isPending}
          >
            <HugeiconsIcon
              icon={FavouriteIcon}
              className={cn("h-5 w-5", recipe.isFavorite && "text-red-500")}
            />
          </Button>
          <Button variant="outline" onClick={handleEdit}>
            <HugeiconsIcon icon={Edit01Icon} className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button variant="destructive" onClick={handleDelete}>
            <HugeiconsIcon icon={Delete01Icon} className="w-4 h-4 mr-2" />
            Delete
          </Button>
        </div>
      </div>

      {/* Image */}
      <div className="group relative max-w-sm overflow-hidden rounded-lg">
        <img
          src={recipe.imageUrl ?? placeholderImage}
          alt={recipe.title}
          className="aspect-square w-full object-cover"
        />
        {recipe.source?.startsWith("http") && (
          <Button
            variant="secondary"
            size="sm"
            className="absolute right-3 bottom-3 opacity-0 shadow-md transition-opacity group-hover:opacity-100"
            onClick={() => setImagePickerOpen(true)}
          >
            <HugeiconsIcon icon={Image02Icon} className="mr-2 h-4 w-4" />
            Change Image
          </Button>
        )}
      </div>

      {/* Meta Info */}
      <div className="flex flex-wrap gap-4">
        {recipe.rating && recipe.rating !== "Not Rated" && (
          <Badge variant="secondary">Rating: {recipe.rating}</Badge>
        )}
        {recipe.servings && (
          <Badge variant="outline">Servings: {recipe.servings}</Badge>
        )}
        {recipe.flags.map((flag) => (
          <Badge key={flag} variant="outline">
            {flag}
          </Badge>
        ))}
        {recipe.tags.map((tag) => (
          <Badge key={tag} variant="secondary">
            {tag}
          </Badge>
        ))}
      </div>

      {/* Description */}
      {recipe.description && (
        <Card>
          <CardHeader>
            <CardTitle>Description</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="whitespace-pre-wrap">{recipe.description}</p>
          </CardContent>
        </Card>
      )}

      {/* Ingredients */}
      {recipe.ingredients && recipe.ingredients.length > 0 && (
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0">
            <CardTitle>
              Ingredients ({recipe.ingredients.length})
            </CardTitle>
            <ScaleInput value={scale} onChange={setScale} />
          </CardHeader>
          <CardContent>
            {(() => {
              const sorted = [...recipe.ingredients].sort((a, b) => a.sortOrder - b.sortOrder);
              let lastGroup: string | null | undefined = undefined;

              return (
                <div className="space-y-1">
                  {sorted.map((ingredient) => {
                    const showGroupHeader = ingredient.groupName !== lastGroup && ingredient.groupName !== null;
                    const isFirstGroup = lastGroup === undefined;
                    lastGroup = ingredient.groupName;

                    const scaledAmount = ingredient.amount != null
                      ? formatScaledAmount(ingredient.amount * scale)
                      : null;

                    return (
                      <div key={ingredient.id}>
                        {showGroupHeader && (
                          <h4
                            className={cn(
                              "border-b pb-1 text-sm font-semibold uppercase tracking-wide text-foreground/70",
                              isFirstGroup ? "mt-0 mb-2" : "mt-4 mb-2"
                            )}
                          >
                            {ingredient.groupName}
                          </h4>
                        )}
                        <li className="flex gap-1 list-none">
                          {scaledAmount != null ? (
                            <span className={cn("font-medium", scale !== 1 && "text-primary")}>
                              {scaledAmount}
                            </span>
                          ) : ingredient.amountText ? (
                            <span className="font-medium">
                              {ingredient.amountText}
                            </span>
                          ) : null}
                          {ingredient.unit && (
                            <span className="text-muted-foreground">
                              {formatUnit(ingredient.unit, ingredient.amount != null ? ingredient.amount * scale : null)}
                            </span>
                          )}
                          <span>{ingredient.name ?? ingredient.rawText}</span>
                        </li>
                      </div>
                    );
                  })}
                </div>
              );
            })()}
          </CardContent>
        </Card>
      )}

      {/* Steps */}
      {recipe.steps && (
        <Card>
          <CardHeader>
            <CardTitle>Instructions</CardTitle>
          </CardHeader>
          <CardContent>
            <StepViewer steps={recipe.steps} />
          </CardContent>
        </Card>
      )}

      {/* Notes */}
      {recipe.notes && (
        <Card>
          <CardHeader>
            <CardTitle>Notes</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="whitespace-pre-wrap">{recipe.notes}</p>
          </CardContent>
        </Card>
      )}

      {/* Nutrition Info */}
      {recipe.nutritionInfo && (
        <Card>
          <CardHeader>
            <CardTitle>Nutrition Information</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
              {recipe.nutritionInfo.calories && (
                <div>
                  <p className="text-sm text-muted-foreground">Calories</p>
                  <p className="font-medium">{recipe.nutritionInfo.calories}</p>
                </div>
              )}
              {recipe.nutritionInfo.proteinGrams && (
                <div>
                  <p className="text-sm text-muted-foreground">Protein</p>
                  <p className="font-medium">
                    {recipe.nutritionInfo.proteinGrams}g
                  </p>
                </div>
              )}
              {recipe.nutritionInfo.totalCarbohydratesGrams && (
                <div>
                  <p className="text-sm text-muted-foreground">Carbs</p>
                  <p className="font-medium">
                    {recipe.nutritionInfo.totalCarbohydratesGrams}g
                  </p>
                </div>
              )}
              {recipe.nutritionInfo.totalFatGrams && (
                <div>
                  <p className="text-sm text-muted-foreground">Fat</p>
                  <p className="font-medium">
                    {recipe.nutritionInfo.totalFatGrams}g
                  </p>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Recipe</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete "{recipe.title}"? This action
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteRecipe.isPending ? "Deleting..." : "Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Source Image Picker */}
      {recipe.source?.startsWith("http") && (
        <SourceImagePicker
          recipeId={id}
          source={recipe.source}
          open={imagePickerOpen}
          onOpenChange={setImagePickerOpen}
        />
      )}
    </div>
  );
}
