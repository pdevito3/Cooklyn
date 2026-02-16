import { useState, useCallback } from 'react'
import Cropper from 'react-easy-crop'
import type { Area } from 'react-easy-crop'

import { Button } from '@/components/ui/button'
import {
  ResponsiveDialog,
  ResponsiveDialogContent,
  ResponsiveDialogHeader,
  ResponsiveDialogTitle,
  ResponsiveDialogDescription,
  ResponsiveDialogFooter,
} from '@/components/ui/responsive-dialog'
import { cropImage } from '@/lib/crop-image'

interface ImageCropDialogProps {
  imageSrc: string
  open: boolean
  onOpenChange: (open: boolean) => void
  onCropComplete: (file: File) => void
}

export function ImageCropDialog({
  imageSrc,
  open,
  onOpenChange,
  onCropComplete,
}: ImageCropDialogProps) {
  const [crop, setCrop] = useState({ x: 0, y: 0 })
  const [zoom, setZoom] = useState(1)
  const [croppedAreaPixels, setCroppedAreaPixels] = useState<Area | null>(null)
  const [isApplying, setIsApplying] = useState(false)

  const handleCropComplete = useCallback((_: Area, areaPixels: Area) => {
    setCroppedAreaPixels(areaPixels)
  }, [])

  const handleApply = async () => {
    if (!croppedAreaPixels) return
    setIsApplying(true)
    try {
      const file = await cropImage(imageSrc, croppedAreaPixels)
      onCropComplete(file)
      onOpenChange(false)
    } finally {
      setIsApplying(false)
    }
  }

  const handleOpenChange = (nextOpen: boolean) => {
    if (!nextOpen) {
      setCrop({ x: 0, y: 0 })
      setZoom(1)
      setCroppedAreaPixels(null)
    }
    onOpenChange(nextOpen)
  }

  return (
    <ResponsiveDialog open={open} onOpenChange={handleOpenChange}>
      <ResponsiveDialogContent className="md:max-w-lg">
        <ResponsiveDialogHeader>
          <ResponsiveDialogTitle>Crop Image</ResponsiveDialogTitle>
          <ResponsiveDialogDescription>
            Drag to reposition and scroll to zoom.
          </ResponsiveDialogDescription>
        </ResponsiveDialogHeader>

        <div className="relative h-[300px] w-full rounded-lg bg-muted">
          <Cropper
            image={imageSrc}
            crop={crop}
            zoom={zoom}
            aspect={1}
            onCropChange={setCrop}
            onCropComplete={handleCropComplete}
            onZoomChange={setZoom}
          />
        </div>

        <div className="flex items-center gap-3 px-1">
          <span className="text-xs text-muted-foreground">Zoom</span>
          <input
            type="range"
            min={1}
            max={3}
            step={0.01}
            value={zoom}
            onChange={(e) => setZoom(Number(e.target.value))}
            className="h-1.5 w-full cursor-pointer appearance-none rounded-full bg-muted accent-primary"
          />
        </div>

        <ResponsiveDialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => handleOpenChange(false)}
            disabled={isApplying}
          >
            Cancel
          </Button>
          <Button
            type="button"
            onClick={handleApply}
            disabled={!croppedAreaPixels || isApplying}
          >
            {isApplying ? 'Cropping...' : 'Apply Crop'}
          </Button>
        </ResponsiveDialogFooter>
      </ResponsiveDialogContent>
    </ResponsiveDialog>
  )
}
