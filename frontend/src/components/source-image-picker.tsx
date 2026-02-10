import { useEffect, useState } from 'react'

import { ImageCropDialog } from '@/components/image-crop-dialog'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
  SheetFooter,
} from '@/components/ui/sheet'
import {
  useImportRecipePreview,
  useUploadRecipeImage,
  proxyImageUrl,
} from '@/domain/recipes'
import { apiClient } from '@/lib/api-client'
import type { ImportImageDto } from '@/domain/recipes'
import { cn } from '@/lib/utils'

interface SourceImagePickerProps {
  recipeId: string
  source: string
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function SourceImagePicker({
  recipeId,
  source,
  open,
  onOpenChange,
}: SourceImagePickerProps) {
  const [sourceImages, setSourceImages] = useState<ImportImageDto[]>([])
  const [selectedImageUrl, setSelectedImageUrl] = useState<string | null>(null)
  const [cropDialogOpen, setCropDialogOpen] = useState(false)
  const [proxiedImageUrl, setProxiedImageUrl] = useState<string | null>(null)
  const [isFetchingImage, setIsFetchingImage] = useState(false)

  const importPreview = useImportRecipePreview()
  const uploadImage = useUploadRecipeImage()

  useEffect(() => {
    if (!open) return
    setSelectedImageUrl(null)
    setSourceImages([])
    importPreview.mutate(source, {
      onSuccess: (data) => {
        setSourceImages(data.images)
        if (data.images.length > 0) {
          setSelectedImageUrl(data.images[0].url)
        }
      },
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, source])

  const handleConfirm = async () => {
    if (!selectedImageUrl) return
    setIsFetchingImage(true)
    try {
      const response = await apiClient.get(proxyImageUrl(selectedImageUrl), {
        responseType: 'blob',
      })
      const blobUrl = URL.createObjectURL(response.data as Blob)
      setProxiedImageUrl(blobUrl)
      setCropDialogOpen(true)
    } finally {
      setIsFetchingImage(false)
    }
  }

  const handleCropComplete = (croppedFile: File) => {
    if (proxiedImageUrl) URL.revokeObjectURL(proxiedImageUrl)
    setProxiedImageUrl(null)
    uploadImage.mutate(
      { id: recipeId, file: croppedFile },
      { onSuccess: () => onOpenChange(false) },
    )
  }

  return (
    <>
      <Sheet open={open} onOpenChange={onOpenChange}>
        <SheetContent className="overflow-y-auto sm:max-w-lg">
          <SheetHeader>
            <SheetTitle>Choose Image from Source</SheetTitle>
            <SheetDescription>
              Select an image from the recipe source website.
            </SheetDescription>
          </SheetHeader>
          <div className="px-4">
            {importPreview.isPending && (
              <div className="grid grid-cols-2 gap-3">
                {Array.from({ length: 4 }).map((_, i) => (
                  <Skeleton key={i} className="aspect-video rounded-lg" />
                ))}
              </div>
            )}
            {importPreview.isError && (
              <p className="text-sm text-destructive">
                Failed to load images from source.
              </p>
            )}
            {!importPreview.isPending && sourceImages.length === 0 && !importPreview.isError && (
              <p className="py-8 text-center text-sm text-muted-foreground">
                No images found on the source page.
              </p>
            )}
            {sourceImages.length > 0 && (
              <div className="grid grid-cols-2 gap-3">
                {sourceImages.slice(0, 12).map((image) => (
                  <button
                    key={image.url}
                    type="button"
                    onClick={() =>
                      setSelectedImageUrl(
                        selectedImageUrl === image.url ? null : image.url,
                      )
                    }
                    className={cn(
                      'relative aspect-video overflow-hidden rounded-lg border-2 transition-all',
                      selectedImageUrl === image.url
                        ? 'border-primary ring-2 ring-primary/20'
                        : 'border-border hover:border-primary/50',
                    )}
                  >
                    <img
                      src={image.url}
                      alt={image.alt ?? 'Recipe image'}
                      className="h-full w-full object-cover"
                      loading="lazy"
                      onError={(e) => {
                        (e.target as HTMLImageElement).style.display = 'none'
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
            )}
          </div>
          <SheetFooter>
            <Button
              onClick={handleConfirm}
              disabled={!selectedImageUrl || uploadImage.isPending || isFetchingImage}
            >
              {isFetchingImage ? 'Loading...' : uploadImage.isPending ? 'Uploading...' : 'Use This Image'}
            </Button>
          </SheetFooter>
        </SheetContent>
      </Sheet>

      {proxiedImageUrl && (
        <ImageCropDialog
          imageSrc={proxiedImageUrl}
          open={cropDialogOpen}
          onOpenChange={(nextOpen) => {
            setCropDialogOpen(nextOpen)
            if (!nextOpen && proxiedImageUrl) {
              URL.revokeObjectURL(proxiedImageUrl)
              setProxiedImageUrl(null)
            }
          }}
          onCropComplete={handleCropComplete}
        />
      )}
    </>
  )
}
