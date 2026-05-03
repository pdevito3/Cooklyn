import { useState } from 'react'
import { RatingIcon } from '@/components/rating-icon'
import { RecipeSourceActionSheet } from '@/components/recipe-source-action-sheet'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useIsMobile } from '@/hooks/use-mobile'
import { useLongPress } from '@/hooks/use-long-press'
import type { RecipeSummaryDto } from '@/domain/recipes/types'
import { formatSourceDisplay, isSourceUrl } from '@/domain/recipes/utils/source'
import {
  buildSourceDomainFilter,
  getRecipeSourceDomain,
} from '@/domain/recipes/utils/source-filter'
import {
  Calendar03Icon,
  Delete01Icon,
  Edit01Icon,
  FilterIcon,
  LinkSquare02Icon,
  MoreVerticalIcon,
  ShoppingCart01Icon,
  SpoonAndForkIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { Link, useNavigate } from '@tanstack/react-router'

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
  const navigate = useNavigate()
  const isMobile = useIsMobile()
  const sourceDomain = getRecipeSourceDomain(recipe.source)
  const [sourceSheetOpen, setSourceSheetOpen] = useState(false)

  const goToDomainFilter = (domain: string) => {
    navigate({
      to: '/recipes',
      search: { filter: buildSourceDomainFilter(domain) },
    })
  }

  const longPressHandlers = useLongPress({
    onLongPress: () => {
      if (isMobile && sourceDomain) setSourceSheetOpen(true)
    },
  })

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
          {recipe.source &&
            (isSourceUrl(recipe.source) ? (
              <a
                href={recipe.source}
                target="_blank"
                rel="noopener noreferrer"
                onClick={(e) => e.stopPropagation()}
                onContextMenu={
                  isMobile && sourceDomain
                    ? (e) => e.preventDefault()
                    : undefined
                }
                {...(isMobile && sourceDomain ? longPressHandlers : {})}
                className="inline-flex min-w-0 items-center gap-1 hover:text-foreground hover:underline"
              >
                <HugeiconsIcon
                  icon={LinkSquare02Icon}
                  className="h-3 w-3 shrink-0"
                />
                <span className="truncate">
                  {formatSourceDisplay(recipe.source)}
                </span>
              </a>
            ) : (
              <span className="truncate">
                {formatSourceDisplay(recipe.source)}
              </span>
            ))}
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
          {sourceDomain && (
            <DropdownMenuItem onClick={() => goToDomainFilter(sourceDomain)}>
              <HugeiconsIcon icon={FilterIcon} className="mr-2 h-4 w-4" />
              Filter by {sourceDomain}
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

      {sourceDomain && recipe.source && (
        <RecipeSourceActionSheet
          open={sourceSheetOpen}
          onOpenChange={setSourceSheetOpen}
          source={recipe.source}
          domain={sourceDomain}
          onFilterByDomain={() => goToDomainFilter(sourceDomain)}
        />
      )}
    </div>
  )
}
