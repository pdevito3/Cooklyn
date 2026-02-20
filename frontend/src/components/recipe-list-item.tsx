import { RatingIcon } from '@/components/rating-icon'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import type { RecipeSummaryDto } from '@/domain/recipes/types'
import {
  Calendar03Icon,
  Delete01Icon,
  Edit01Icon,
  MoreVerticalIcon,
  ShoppingCart01Icon,
  SpoonAndForkIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { Link } from '@tanstack/react-router'

interface RecipeListItemProps {
  recipe: RecipeSummaryDto
  onEdit?: (id: string) => void
  onDelete?: (id: string) => void
  onAddToShoppingList?: (id: string) => void
  onAddToMealPlan?: (id: string) => void
}

export function RecipeListItem({
  recipe,
  onEdit,
  onDelete,
  onAddToShoppingList,
  onAddToMealPlan,
}: RecipeListItemProps) {
  return (
    <div className="group relative flex items-center gap-4 overflow-hidden rounded-lg border bg-card p-3 transition-colors hover:bg-accent/50">
      {/* Thumbnail */}
      <Link
        to="/recipes/$id"
        params={{ id: recipe.id }}
        className="shrink-0"
      >
        {recipe.imageUrl ? (
          <img
            src={recipe.imageUrl}
            alt={recipe.title}
            className="h-16 w-16 rounded-md object-cover"
          />
        ) : (
          <div className="flex h-16 w-16 items-center justify-center rounded-md bg-muted">
            <HugeiconsIcon
              icon={SpoonAndForkIcon}
              className="size-6 text-muted-foreground"
            />
          </div>
        )}
      </Link>

      {/* Content */}
      <div className="min-w-0 flex-1">
        <Link to="/recipes/$id" params={{ id: recipe.id }} className="block">
          <h3 className="truncate font-semibold transition-colors hover:text-primary">
            {recipe.title}
          </h3>
        </Link>
        {recipe.description && (
          <p className="mt-0.5 truncate text-sm text-muted-foreground">
            {recipe.description}
          </p>
        )}
        <div className="mt-1 flex items-center gap-3 text-xs text-muted-foreground">
          {recipe.servings && <span>{recipe.servings} servings</span>}
          {recipe.ingredientCount > 0 && (
            <span>{recipe.ingredientCount} ingredients</span>
          )}
          {recipe.tags.length > 0 && (
            <span className="truncate">{recipe.tags.slice(0, 3).join(', ')}</span>
          )}
        </div>
      </div>

      {/* Rating */}
      {recipe.rating && recipe.rating !== 'Not Rated' && (
        <RatingIcon rating={recipe.rating} size="md" />
      )}

      {/* Actions */}
      <DropdownMenu>
        <DropdownMenuTrigger
          render={
            <Button
              variant="ghost"
              size="icon"
              className="h-8 w-8 shrink-0 opacity-0 group-hover:opacity-100"
            >
              <HugeiconsIcon
                icon={MoreVerticalIcon}
                className="h-4 w-4"
              />
              <span className="sr-only">Open menu</span>
            </Button>
          }
        />
        <DropdownMenuContent align="end" className="w-auto">
          {onEdit && (
            <DropdownMenuItem onClick={() => onEdit(recipe.id)}>
              <HugeiconsIcon icon={Edit01Icon} className="mr-2 h-4 w-4" />
              Edit
            </DropdownMenuItem>
          )}
          {onAddToShoppingList && (
            <DropdownMenuItem onClick={() => onAddToShoppingList(recipe.id)}>
              <HugeiconsIcon
                icon={ShoppingCart01Icon}
                className="mr-2 h-4 w-4"
              />
              Add to Shopping List
            </DropdownMenuItem>
          )}
          {onAddToMealPlan && (
            <DropdownMenuItem onClick={() => onAddToMealPlan(recipe.id)}>
              <HugeiconsIcon icon={Calendar03Icon} className="mr-2 h-4 w-4" />
              Add to Meal Plan
            </DropdownMenuItem>
          )}
          {onDelete && (
            <>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => onDelete(recipe.id)}
              >
                <HugeiconsIcon icon={Delete01Icon} className="mr-2 h-4 w-4" />
                Delete
              </DropdownMenuItem>
            </>
          )}
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}
