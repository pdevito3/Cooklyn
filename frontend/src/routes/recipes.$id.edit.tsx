import { useRef, useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ArrowLeft02Icon, ImageUploadIcon, Delete02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  useRecipe,
  useUpdateRecipe,
  useUploadRecipeImage,
  useDeleteRecipeImage,
} from '@/domain/recipes'
import { RecipeForm, type RecipeFormValues } from '@/components/recipe-form'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'

export const Route = createFileRoute('/recipes/$id/edit')({
  component: EditRecipePage,
})

function RecipeImageSection({ recipeId, imageUrl }: { recipeId: string; imageUrl: string | null }) {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const uploadImage = useUploadRecipeImage()
  const deleteImage = useDeleteRecipeImage()

  const displayUrl = previewUrl ?? imageUrl
  const isUploading = uploadImage.isPending
  const isDeleting = deleteImage.isPending

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

    // Show local preview immediately
    const objectUrl = URL.createObjectURL(file)
    setPreviewUrl(objectUrl)

    uploadImage.mutate(
      { id: recipeId, file },
      {
        onSuccess: () => {
          setPreviewUrl(null)
          URL.revokeObjectURL(objectUrl)
        },
        onError: () => {
          setPreviewUrl(null)
          URL.revokeObjectURL(objectUrl)
        },
      }
    )

    // Reset input so the same file can be re-selected
    e.target.value = ''
  }

  const handleRemoveImage = () => {
    deleteImage.mutate(recipeId, {
      onSuccess: () => {
        setPreviewUrl(null)
      },
    })
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Recipe Image</CardTitle>
        <CardDescription>Upload an image for this recipe</CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        {displayUrl ? (
          <div className="relative overflow-hidden rounded-lg border">
            <img
              src={displayUrl}
              alt="Recipe"
              className="h-64 w-full object-cover"
            />
            {(isUploading || isDeleting) && (
              <div className="absolute inset-0 flex items-center justify-center bg-background/60">
                <div className="size-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
              </div>
            )}
          </div>
        ) : (
          <div className="flex h-64 items-center justify-center rounded-lg border border-dashed">
            <div className="text-center text-muted-foreground">
              <HugeiconsIcon icon={ImageUploadIcon} className="mx-auto size-10" />
              <p className="mt-2 text-sm">No image uploaded</p>
            </div>
          </div>
        )}

        <input
          ref={fileInputRef}
          type="file"
          accept="image/*"
          className="hidden"
          onChange={handleFileChange}
        />

        <div className="flex gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() => fileInputRef.current?.click()}
            disabled={isUploading || isDeleting}
          >
            <HugeiconsIcon icon={ImageUploadIcon} className="mr-2 size-4" />
            {displayUrl ? 'Replace Image' : 'Upload Image'}
          </Button>
          {imageUrl && !isUploading && (
            <Button
              type="button"
              variant="outline"
              onClick={handleRemoveImage}
              disabled={isDeleting}
            >
              <HugeiconsIcon icon={Delete02Icon} className="mr-2 size-4" />
              Remove Image
            </Button>
          )}
        </div>

        {uploadImage.isError && (
          <p className="text-sm text-destructive">
            {uploadImage.error instanceof Error
              ? uploadImage.error.message
              : 'Failed to upload image'}
          </p>
        )}
        {deleteImage.isError && (
          <p className="text-sm text-destructive">
            {deleteImage.error instanceof Error
              ? deleteImage.error.message
              : 'Failed to remove image'}
          </p>
        )}
      </CardContent>
    </Card>
  )
}

function EditRecipePage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()

  const { data: recipe, isLoading, error } = useRecipe(id)
  const updateRecipe = useUpdateRecipe()

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
          navigate({ to: '/recipes/$id', params: { id } })
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
        <RecipeImageSection recipeId={id} imageUrl={recipe.imageUrl} />

        <RecipeForm
          existingRecipe={recipe}
          onSubmit={handleSubmit}
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
