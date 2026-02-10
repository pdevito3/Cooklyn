import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ArrowLeft02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  useRecipe,
  useUpdateRecipe,
  useUpdateRecipeIngredients,
} from '@/domain/recipes'
import { RecipeForm, type RecipeFormValues } from '@/components/recipe-form'
import { RecipeImageSection } from '@/components/recipe-image-section'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'

export const Route = createFileRoute('/recipes/$id/edit')({
  component: EditRecipePage,
})

function EditRecipePage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()

  const { data: recipe, isLoading, error } = useRecipe(id)
  const updateRecipe = useUpdateRecipe()
  const updateIngredients = useUpdateRecipeIngredients()

  const handleSubmit = (values: RecipeFormValues) => {
    updateRecipe.mutate(
      {
        id,
        dto: {
          title: values.title,
          description: values.description,
          imageS3Bucket: recipe?.imageS3Bucket ?? null,
          imageS3Key: recipe?.imageS3Key ?? null,
          rating: values.rating,
          source: values.source,
          isFavorite: values.isFavorite,
          servings: values.servings,
          steps: values.steps,
          notes: values.notes,
        },
      },
      {
        onSuccess: () => {
          updateIngredients.mutate(
            { id, ingredients: values.ingredients },
            {
              onSuccess: () => {
                navigate({ to: '/recipes/$id', params: { id } })
              },
              onError: () => {
                navigate({ to: '/recipes/$id', params: { id } })
              },
            }
          )
        },
      }
    )
  }

  const handleBack = () => {
    navigate({ to: '/recipes/$id', params: { id } })
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10" />
          <Skeleton className="h-8 w-64" />
        </div>
        <div className="mx-auto max-w-3xl space-y-6">
          <Skeleton className="h-48 w-full rounded-lg" />
          <Skeleton className="h-48 w-full rounded-lg" />
          <Skeleton className="h-48 w-full rounded-lg" />
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={handleBack}>
            <HugeiconsIcon icon={ArrowLeft02Icon} className="h-5 w-5" />
          </Button>
          <h1 className="text-3xl font-bold tracking-tight">Recipe Not Found</h1>
        </div>
        <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
          <p className="font-medium">Error loading recipe</p>
          <p className="mt-1 text-sm">
            {error instanceof Error ? error.message : 'Failed to load recipe'}
          </p>
        </div>
        <Button onClick={() => navigate({ to: '/recipes' })}>Back to Recipes</Button>
      </div>
    )
  }

  if (!recipe) {
    return null
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={handleBack}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-5 w-5" />
        </Button>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Edit Recipe</h1>
          <p className="text-muted-foreground">
            Editing: {recipe.title}
          </p>
        </div>
      </div>

      {/* Image Upload + Form */}
      <div className="mx-auto max-w-3xl space-y-6">
        <RecipeImageSection recipeId={id} imageUrl={recipe.imageUrl} source={recipe.source} />

        <RecipeForm
          existingRecipe={recipe}
          onSubmit={handleSubmit}
          onCancel={handleBack}
          isSubmitting={updateRecipe.isPending}
          submitLabel="Update Recipe"
        />

        {updateRecipe.isError && (
          <div className="mt-4 rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
            <p className="font-medium">Error updating recipe</p>
            <p className="mt-1 text-sm">
              {updateRecipe.error instanceof Error
                ? updateRecipe.error.message
                : 'An unexpected error occurred'}
            </p>
          </div>
        )}
      </div>
    </div>
  )
}
