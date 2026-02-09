import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { HugeiconsIcon } from '@hugeicons/react'
import { FavouriteIcon } from '@hugeicons/core-free-icons'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Checkbox } from '@/components/ui/checkbox'
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
import {
  RECIPE_RATINGS,
  RECIPE_FLAGS,
  type RecipeDto,
  type IngredientForCreationDto,
} from '@/domain/recipes'
import { cn } from '@/lib/utils'

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
  isFavorite: z.boolean(),
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
}: RecipeFormProps) {
  // Transform existing recipe to form values
  const initialValues: RecipeFormValues = existingRecipe
    ? {
        title: existingRecipe.title,
        description: existingRecipe.description,
        rating: existingRecipe.rating === 'Not Rated' ? null : existingRecipe.rating,
        source: existingRecipe.source,
        isFavorite: existingRecipe.isFavorite,
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
        isFavorite: defaultValues?.isFavorite ?? false,
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
    watch,
    formState: { errors },
  } = useForm<RecipeFormValues>({
    resolver: zodResolver(recipeFormSchema),
    defaultValues: initialValues,
  })

  const isFavorite = watch('isFavorite')

  const formButtons = (
    <div className="flex justify-end gap-4">
      {onCancel && (
        <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </Button>
      )}
      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Saving...' : submitLabel}
      </Button>
    </div>
  )

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      {formButtons}

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
          <div className="grid gap-4 sm:grid-cols-2">
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
                          <SelectItemText>{option.label}</SelectItemText>
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

            <div className="flex flex-col justify-end gap-2">
              <Controller
                control={control}
                name="isFavorite"
                render={({ field }) => (
                  <Checkbox
                    isSelected={field.value}
                    onChange={field.onChange}
                  >
                    <HugeiconsIcon
                      icon={FavouriteIcon}
                      className={cn('h-4 w-4', isFavorite && 'text-red-500')}
                    />
                    Mark as favorite
                  </Checkbox>
                )}
              />
            </div>
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

      {formButtons}
    </form>
  )
}
