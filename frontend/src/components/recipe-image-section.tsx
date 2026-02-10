import { useRef, useState } from 'react'
import { ImageUploadIcon, Delete02Icon, Image02Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { SourceImagePicker } from '@/components/source-image-picker'
import {
  useUploadRecipeImage,
  useDeleteRecipeImage,
} from '@/domain/recipes'

interface RecipeImageSectionProps {
  recipeId: string
  imageUrl: string | null
  source: string | null
}

export function RecipeImageSection({ recipeId, imageUrl, source }: RecipeImageSectionProps) {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [imagePickerOpen, setImagePickerOpen] = useState(false)

  const uploadImage = useUploadRecipeImage()
  const deleteImage = useDeleteRecipeImage()

  const displayUrl = previewUrl ?? imageUrl
  const isUploading = uploadImage.isPending
  const isDeleting = deleteImage.isPending
  const hasSourceUrl = source?.startsWith('http')

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return

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
    <>
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

          <div className="flex flex-wrap gap-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => fileInputRef.current?.click()}
              disabled={isUploading || isDeleting}
            >
              <HugeiconsIcon icon={ImageUploadIcon} className="mr-2 size-4" />
              {displayUrl ? 'Replace Image' : 'Upload Image'}
            </Button>
            {hasSourceUrl && (
              <Button
                type="button"
                variant="outline"
                onClick={() => setImagePickerOpen(true)}
                disabled={isUploading || isDeleting}
              >
                <HugeiconsIcon icon={Image02Icon} className="mr-2 size-4" />
                Pick from Source
              </Button>
            )}
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

      {hasSourceUrl && (
        <SourceImagePicker
          recipeId={recipeId}
          source={source!}
          open={imagePickerOpen}
          onOpenChange={setImagePickerOpen}
        />
      )}
    </>
  )
}
