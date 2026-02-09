import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ArrowLeft02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { useCreateRecipe } from '@/domain/recipes'
import { RecipeForm, type RecipeFormValues } from '@/components/recipe-form'
import { Button } from '@/components/ui/button'

export const Route = createFileRoute('/recipes/new')({
  component: NewRecipePage,
})

function NewRecipePage() {
  const navigate = useNavigate()
  const createRecipe = useCreateRecipe()

  const handleSubmit = (values: RecipeFormValues) => {
    createRecipe.mutate(
      {
        title: values.title,
        description: values.description,
        imageS3Bucket: null,
        imageS3Key: null,
        rating: values.rating,
        source: values.source,
        isFavorite: values.isFavorite,
        servings: values.servings,
        steps: values.steps,
        notes: values.notes,
        tagIds: [],
        flags: values.flags.map((f) => f.value),
        ingredients: values.ingredients,
        nutritionInfo: null,
      },
      {
        onSuccess: (recipe) => {
          navigate({ to: '/recipes/$id', params: { id: recipe.id } })
        },
      }
    )
  }

  const handleBack = () => {
    navigate({ to: '/recipes' })
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={handleBack}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-5 w-5" />
        </Button>
        <div>
          <h1 className="text-3xl font-bold tracking-tight">New Recipe</h1>
          <p className="text-muted-foreground">
            Create a new recipe for your collection
          </p>
        </div>
      </div>

      {/* Form */}
      <div className="mx-auto max-w-3xl">
        <RecipeForm
          onSubmit={handleSubmit}
          onCancel={handleBack}
          isSubmitting={createRecipe.isPending}
          submitLabel="Create Recipe"
        />

        {createRecipe.isError && (
          <div className="mt-4 rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
            <p className="font-medium">Error creating recipe</p>
            <p className="mt-1 text-sm">
              {createRecipe.error instanceof Error
                ? createRecipe.error.message
                : 'An unexpected error occurred'}
            </p>
          </div>
        )}
      </div>
    </div>
  )
}
