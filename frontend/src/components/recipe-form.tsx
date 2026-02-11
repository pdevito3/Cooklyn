import { useEffect, useMemo, useRef, useState } from 'react'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HugeiconsIcon } from '@hugeicons/react'
import { ImageUploadIcon, Delete02Icon } from '@hugeicons/core-free-icons'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { MultiSelect, type MultiSelectOption } from '@/components/ui/multi-select'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectItemIndicator,
  SelectItemText,
  SelectTrigger,
  SelectValue,
  SelectIcon,
} from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { IngredientEditor } from '@/components/ingredient-editor'
import { ImageCropDialog } from '@/components/image-crop-dialog'
import {
  RECIPE_RATINGS,
  RECIPE_FLAGS,
  type RecipeDto,
  type IngredientForCreationDto,
} from '@/domain/recipes'
import { RatingIcon } from '@/components/rating-icon'

const ingredientSchema = z.object({
  rawText: z.string(),
  name: z.string().nullable(),
  amount: z.number().nullable(),
  amountText: z.string().nullable(),
  unit: z.string().nullable(),
  customUnit: z.string().nullable(),
  groupName: z.string().nullable(),
  sortOrder: z.number(),
})

const recipeFormSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title is too long'),
  description: z.string().max(2000, 'Description is too long').nullable(),
  rating: z.string().nullable(),
  source: z.string().max(500, 'Source is too long').nullable(),
  servings: z.number().int().min(1).max(999).nullable(),
  steps: z.string().nullable(),
  notes: z.string().nullable(),
  flags: z.array(z.object({ value: z.string(), label: z.string() })),
  ingredients: z.array(ingredientSchema),
})

export type RecipeFormValues = z.infer<typeof recipeFormSchema>

interface RecipeFormProps {
  defaultValues?: Partial<RecipeFormValues>
  existingRecipe?: RecipeDto
  onSubmit: (values: RecipeFormValues) => void
  onCancel?: () => void
  isSubmitting?: boolean
  submitLabel?: string
  imageFile?: File | null
  onImageFileChange?: (file: File | null) => void
}

const flagOptions: MultiSelectOption[] = RECIPE_FLAGS.map((flag) => ({
  value: flag,
  label: flag,
}))

const ratingOptions = RECIPE_RATINGS.map((rating) => ({
  value: rating,
  label: rating,
}))

export function RecipeForm({
  defaultValues,
  existingRecipe,
  onSubmit,
  onCancel,
  isSubmitting = false,
  submitLabel = 'Save Recipe',
  imageFile,
  onImageFileChange,
}: RecipeFormProps) {
  const imageInputRef = useRef<HTMLInputElement>(null)
  const [cropDialogOpen, setCropDialogOpen] = useState(false)
  const [rawImageUrl, setRawImageUrl] = useState<string | null>(null)
  const imagePreviewUrl = useMemo(
    () => (imageFile ? URL.createObjectURL(imageFile) : null),
    [imageFile]
  )
  useEffect(() => {
    return () => {
      if (imagePreviewUrl) URL.revokeObjectURL(imagePreviewUrl)
    }
  }, [imagePreviewUrl])
  // Transform existing recipe to form values
  const initialValues: RecipeFormValues = existingRecipe
    ? {
        title: existingRecipe.title,
        description: existingRecipe.description,
        rating: existingRecipe.rating === 'Not Rated' ? null : existingRecipe.rating,
        source: existingRecipe.source,
        servings: existingRecipe.servings,
        steps: existingRecipe.steps,
        notes: existingRecipe.notes,
        flags: existingRecipe.flags.map((f) => ({ value: f, label: f })),
        ingredients: (existingRecipe.ingredients ?? []).map((i) => ({
          rawText: i.rawText,
          name: i.name,
          amount: i.amount,
          amountText: i.amountText,
          unit: i.unit,
          customUnit: i.customUnit,
          groupName: i.groupName,
          sortOrder: i.sortOrder,
        })),
      }
    : {
        title: defaultValues?.title ?? '',
        description: defaultValues?.description ?? null,
        rating: defaultValues?.rating ?? null,
        source: defaultValues?.source ?? null,
        servings: defaultValues?.servings ?? null,
        steps: defaultValues?.steps ?? null,
        notes: defaultValues?.notes ?? null,
        flags: defaultValues?.flags ?? [],
        ingredients: defaultValues?.ingredients ?? [],
      }

  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<RecipeFormValues>({
    resolver: zodResolver(recipeFormSchema),
    defaultValues: initialValues,
  })

  const actionButtons = (
    <>
      {onCancel && (
        <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
      )}
      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Saving...' : submitLabel}
      </Button>
    </>
  )

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="@container space-y-6">
      <div className="flex justify-end gap-4">
        {actionButtons}
      </div>

      {/* Basic Information */}
      <Card>
        <CardHeader>
          <CardTitle>Basic Information</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-2">
            <Label htmlFor="title">Title *</Label>
            <Input
              id="title"
              placeholder="Enter recipe title"
              {...register('title')}
              aria-invalid={!!errors.title}
            />
            {errors.title && (
              <p className="text-sm font-medium text-destructive">{errors.title.message}</p>
            )}
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Brief description of the recipe"
              {...register('description')}
              aria-invalid={!!errors.description}
            />
            {errors.description && (
              <p className="text-sm font-medium text-destructive">{errors.description.message}</p>
            )}
          </div>

          {onImageFileChange && (
            <div className="flex flex-col gap-2">
              <Label>Image</Label>
              {imagePreviewUrl ? (
                <div className="relative overflow-hidden rounded-lg border">
                  <img
                    src={imagePreviewUrl}
                    alt="Recipe preview"
                    className="h-48 w-full object-cover"
                  />
                </div>
              ) : (
                <div
                  className="flex h-48 cursor-pointer items-center justify-center rounded-lg border border-dashed transition-colors hover:border-primary/50 hover:bg-muted/50"
                  onClick={() => imageInputRef.current?.click()}
                >
                  <div className="text-center text-muted-foreground">
                    <HugeiconsIcon icon={ImageUploadIcon} className="mx-auto size-8" />
                    <p className="mt-2 text-sm">Click to add an image</p>
                  </div>
                </div>
              )}
              <input
                ref={imageInputRef}
                type="file"
                accept="image/*"
                className="hidden"
                onChange={(e) => {
                  const file = e.target.files?.[0]
                  if (file) {
                    const url = URL.createObjectURL(file)
                    setRawImageUrl(url)
                    setCropDialogOpen(true)
                  }
                  e.target.value = ''
                }}
              />
              {imageFile && (
                <div className="flex gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={() => imageInputRef.current?.click()}
                  >
                    <HugeiconsIcon icon={ImageUploadIcon} className="mr-2 size-4" />
                    Replace
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    onClick={() => onImageFileChange(null)}
                  >
                    <HugeiconsIcon icon={Delete02Icon} className="mr-2 size-4" />
                    Remove
                  </Button>
                </div>
              )}
              {rawImageUrl && (
                <ImageCropDialog
                  imageSrc={rawImageUrl}
                  open={cropDialogOpen}
                  onOpenChange={(open) => {
                    setCropDialogOpen(open)
                    if (!open) {
                      URL.revokeObjectURL(rawImageUrl)
                      setRawImageUrl(null)
                    }
                  }}
                  onCropComplete={(croppedFile) => {
                    onImageFileChange(croppedFile)
                    if (rawImageUrl) URL.revokeObjectURL(rawImageUrl)
                    setRawImageUrl(null)
                  }}
                />
              )}
            </div>
          )}

          <div className="grid gap-4 sm:grid-cols-2">
            <div className="flex flex-col gap-2">
              <Label htmlFor="servings">Servings</Label>
              <Controller
                control={control}
                name="servings"
                render={({ field }) => (
                  <Input
                    id="servings"
                    type="number"
                    placeholder="Number of servings"
                    min={1}
                    max={999}
                    value={field.value ?? ''}
                    onChange={(e) => {
                      const val = e.target.value
                      field.onChange(val === '' ? null : parseInt(val, 10))
                    }}
                    aria-invalid={!!errors.servings}
                  />
                )}
              />
              {errors.servings && (
                <p className="text-sm font-medium text-destructive">{errors.servings.message}</p>
              )}
            </div>

            <div className="flex flex-col gap-2">
              <Label htmlFor="source">Source</Label>
              <Input
                id="source"
                placeholder="Recipe source or URL"
                {...register('source')}
                aria-invalid={!!errors.source}
              />
              {errors.source && (
                <p className="text-sm font-medium text-destructive">{errors.source.message}</p>
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Ingredients */}
      <Card>
        <CardHeader>
          <CardTitle>Ingredients</CardTitle>
        </CardHeader>
        <CardContent>
          <Controller
            control={control}
            name="ingredients"
            render={({ field }) => (
              <IngredientEditor
                value={field.value as IngredientForCreationDto[]}
                onChange={field.onChange}
              />
            )}
          />
        </CardContent>
      </Card>

      {/* Recipe Details */}
      <Card>
        <CardHeader>
          <CardTitle>Recipe Details</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-2">
            <Label htmlFor="steps">Instructions</Label>
            <Textarea
              id="steps"
              placeholder="Enter step-by-step instructions..."
              className="min-h-[200px]"
              {...register('steps')}
              aria-invalid={!!errors.steps}
            />
            <p className="text-sm text-muted-foreground">
              Enter each step on a new line or number them (1. First step, 2. Second step, etc.)
            </p>
            {errors.steps && (
              <p className="text-sm font-medium text-destructive">{errors.steps.message}</p>
            )}
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="notes">Notes</Label>
            <Textarea
              id="notes"
              placeholder="Additional notes, tips, or variations..."
              {...register('notes')}
              aria-invalid={!!errors.notes}
            />
            {errors.notes && (
              <p className="text-sm font-medium text-destructive">{errors.notes.message}</p>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Rating and Tags */}
      <Card>
        <CardHeader>
          <CardTitle>Rating and Tags</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-2">
            <Label>Rating</Label>
            <Controller
              control={control}
              name="rating"
              render={({ field }) => (
                <Select
                  value={field.value ?? undefined}
                  onValueChange={(value) => field.onChange(value || null)}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select rating" />
                    <SelectIcon />
                  </SelectTrigger>
                  <SelectContent>
                    {ratingOptions.map((option) => (
                      <SelectItem key={option.value} value={option.value}>
                        <SelectItemIndicator />
                        <SelectItemText>
                          <span className="flex items-center gap-2">
                            <RatingIcon rating={option.value} size="sm" />
                            {option.label}
                          </span>
                        </SelectItemText>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
            {errors.rating && (
              <p className="text-sm font-medium text-destructive">{errors.rating.message}</p>
            )}
          </div>

          <div className="flex flex-col gap-2">
            <Label>Dietary Flags</Label>
            <Controller
              control={control}
              name="flags"
              render={({ field }) => (
                <MultiSelect
                  options={flagOptions}
                  value={field.value}
                  onValueChange={field.onChange}
                  placeholder="Select dietary flags..."
                  maxChips={5}
                />
              )}
            />
            <p className="text-sm text-muted-foreground">
              Select any applicable dietary flags (Vegan, Gluten-Free, etc.)
            </p>
            {errors.flags && (
              <p className="text-sm font-medium text-destructive">{errors.flags.message}</p>
            )}
          </div>
        </CardContent>
      </Card>

      <div className="flex justify-end gap-4 @sm:hidden">
        {actionButtons}
      </div>
    </form>
  )
}
