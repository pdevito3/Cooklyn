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

interface RecipeSmallCardProps {
  recipe: RecipeSummaryDto
  onEdit?: (id: string) => void
  onDelete?: (id: string) => void
  onAddToShoppingList?: (id: string) => void
  onAddToMealPlan?: (id: string) => void
}

export function RecipeSmallCard({
  recipe,
  onEdit,
  onDelete,
  onAddToShoppingList,
  onAddToMealPlan,
}: RecipeSmallCardProps) {
  return (
    <div className="group relative overflow-hidden rounded-lg border bg-card shadow-sm transition-all hover:shadow-md">
      {/* Image */}
      <Link
        to="/recipes/$id"
        params={{ id: recipe.id }}
        className="block aspect-[3/2] overflow-hidden"
      >
        {recipe.imageUrl ? (
          <img
            src={recipe.imageUrl}
            alt={recipe.title}
            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center bg-muted">
            <HugeiconsIcon
              icon={SpoonAndForkIcon}
              className="size-8 text-muted-foreground"
            />
          </div>
        )}
      </Link>

      {/* Top right actions */}
      <div className="absolute right-1.5 top-1.5 z-10 flex items-center gap-1">
        {recipe.rating && recipe.rating !== 'Not Rated' && (
          <span className="flex h-6 w-6 items-center justify-center rounded-full bg-white/90 shadow-sm">
            <RatingIcon rating={recipe.rating} size="sm" />
          </span>
        )}

        <DropdownMenu>
          <DropdownMenuTrigger
            render={
              <Button
                variant="ghost"
                size="icon"
                className="h-6 w-6 bg-white/90 shadow-sm hover:bg-white opacity-0 group-hover:opacity-100"
                onClick={(e) => e.stopPropagation()}
              >
                <HugeiconsIcon
                  icon={MoreVerticalIcon}
                  className="h-3 w-3 text-gray-700"
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
                <HugeiconsIcon
                  icon={Calendar03Icon}
                  className="mr-2 h-4 w-4"
                />
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
                  <HugeiconsIcon
                    icon={Delete01Icon}
                    className="mr-2 h-4 w-4"
                  />
                  Delete
                </DropdownMenuItem>
              </>
            )}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {/* Content */}
      <div className="p-2.5">
        <Link to="/recipes/$id" params={{ id: recipe.id }} className="block">
          <h3 className="truncate text-sm font-semibold leading-tight transition-colors hover:text-primary">
            {recipe.title}
          </h3>
        </Link>
        {recipe.description && (
          <p className="mt-0.5 truncate text-xs text-muted-foreground">
            {recipe.description}
          </p>
        )}
      </div>
    </div>
  )
}
