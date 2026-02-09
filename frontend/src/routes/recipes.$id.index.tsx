import {
  ArrowLeft02Icon,
  Delete01Icon,
  Edit01Icon,
  FavouriteIcon,
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
import {
  useDeleteRecipe,
  useRecipe,
  useToggleRecipeFavorite,
} from "@/domain/recipes";
import { cn } from "@/lib/utils";

export const Route = createFileRoute("/recipes/$id/")({
  component: RecipeDetailPage,
});

function RecipeDetailPage() {
  const { id } = Route.useParams();
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

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
              <p className="text-muted-foreground">Source: {recipe.source}</p>
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
      <div className="overflow-hidden rounded-lg">
        <img
          src={recipe.imageUrl ?? placeholderImage}
          alt={recipe.title}
          className="aspect-[4/3] w-full object-cover"
        />
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
          <CardHeader>
            <CardTitle>
              Ingredients ({recipe.ingredients.length})
            </CardTitle>
          </CardHeader>
          <CardContent>
            {(() => {
              // Group ingredients by groupName
              const grouped = new Map<string, typeof recipe.ingredients>();
              const ungrouped: typeof recipe.ingredients = [];

              for (const ingredient of recipe.ingredients) {
                if (ingredient.groupName) {
                  const existing = grouped.get(ingredient.groupName) ?? [];
                  existing.push(ingredient);
                  grouped.set(ingredient.groupName, existing);
                } else {
                  ungrouped.push(ingredient);
                }
              }

              return (
                <div className="space-y-4">
                  {ungrouped.length > 0 && (
                    <ul className="space-y-1">
                      {ungrouped
                        .sort((a, b) => a.sortOrder - b.sortOrder)
                        .map((ingredient) => (
                          <li key={ingredient.id} className="flex gap-1">
                            {ingredient.amountText && (
                              <span className="font-medium">
                                {ingredient.amountText}
                              </span>
                            )}
                            {ingredient.unit && (
                              <span className="text-muted-foreground">
                                {ingredient.unit}
                              </span>
                            )}
                            <span>{ingredient.name ?? ingredient.rawText}</span>
                          </li>
                        ))}
                    </ul>
                  )}
                  {Array.from(grouped.entries()).map(([groupName, items]) => (
                    <div key={groupName}>
                      <h4 className="mb-1 font-semibold">{groupName}</h4>
                      <ul className="space-y-1">
                        {items
                          .sort((a, b) => a.sortOrder - b.sortOrder)
                          .map((ingredient) => (
                            <li key={ingredient.id} className="flex gap-1">
                              {ingredient.amountText && (
                                <span className="font-medium">
                                  {ingredient.amountText}
                                </span>
                              )}
                              {ingredient.unit && (
                                <span className="text-muted-foreground">
                                  {ingredient.unit}
                                </span>
                              )}
                              <span>
                                {ingredient.name ?? ingredient.rawText}
                              </span>
                            </li>
                          ))}
                      </ul>
                    </div>
                  ))}
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
            <div className="whitespace-pre-wrap">{recipe.steps}</div>
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
    </div>
  );
}
