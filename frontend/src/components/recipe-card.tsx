import { Link } from '@tanstack/react-router'
import { MoreHorizontalIcon, Delete01Icon, Edit01Icon, FavouriteIcon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { Card, CardContent, CardHeader, CardTitle, CardAction } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import type { RecipeSummaryDto } from '@/domain/recipes'
import { cn } from '@/lib/utils'

interface RecipeCardProps {
  recipe: RecipeSummaryDto
  onEdit?: (id: string) => void
  onDelete?: (id: string) => void
  onToggleFavorite?: (id: string) => void
}

export function RecipeCard({
  recipe,
  onEdit,
  onDelete,
  onToggleFavorite,
}: RecipeCardProps) {
  const placeholderImage = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300"%3E%3Crect fill="%23e2e8f0" width="400" height="300"/%3E%3Ctext fill="%2394a3b8" font-family="system-ui" font-size="20" x="50%25" y="50%25" text-anchor="middle" dominant-baseline="middle"%3ENo Image%3C/text%3E%3C/svg%3E'

  return (
    <Card className="h-full overflow-hidden transition-shadow hover:shadow-lg">
      {/* Image */}
      <Link to="/recipes/$id" params={{ id: recipe.id }} className="block">
        <div className="relative aspect-video overflow-hidden">
          <img
            src={recipe.imageUrl ?? placeholderImage}
            alt={recipe.title}
            className="h-full w-full object-cover transition-transform hover:scale-105"
          />
          {recipe.isFavorite && (
            <div className="absolute right-2 top-2">
              <span className="flex h-8 w-8 items-center justify-center rounded-full bg-white/90 shadow-sm">
                <HugeiconsIcon
                  icon={FavouriteIcon}
                  className="h-5 w-5 text-red-500"
                  strokeWidth={2}
                />
              </span>
            </div>
          )}
        </div>
      </Link>

      <CardHeader className="pb-2">
        <CardTitle className="line-clamp-1">
          <Link
            to="/recipes/$id"
            params={{ id: recipe.id }}
            className="hover:underline"
          >
            {recipe.title}
          </Link>
        </CardTitle>
        <CardAction>
          <DropdownMenu>
            <DropdownMenuTrigger
              render={
                <Button variant="ghost" size="icon" className="h-8 w-8">
                  <HugeiconsIcon icon={MoreHorizontalIcon} className="h-4 w-4" />
                  <span className="sr-only">Open menu</span>
                </Button>
              }
            />
            <DropdownMenuContent align="end">
              {onToggleFavorite && (
                <DropdownMenuItem onClick={() => onToggleFavorite(recipe.id)}>
                  <HugeiconsIcon
                    icon={FavouriteIcon}
                    className={cn('mr-2 h-4 w-4', recipe.isFavorite && 'text-red-500')}
                  />
                  {recipe.isFavorite ? 'Remove from favorites' : 'Add to favorites'}
                </DropdownMenuItem>
              )}
              {onEdit && (
                <DropdownMenuItem onClick={() => onEdit(recipe.id)}>
                  <HugeiconsIcon icon={Edit01Icon} className="mr-2 h-4 w-4" />
                  Edit
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
        </CardAction>
      </CardHeader>

      <CardContent className="pb-4">
        {/* Description */}
        {recipe.description && (
          <p className="text-muted-foreground mb-3 line-clamp-2 text-sm">
            {recipe.description}
          </p>
        )}

        {/* Rating */}
        {recipe.rating && recipe.rating !== 'Not Rated' && (
          <div className="text-muted-foreground mb-2 text-sm">
            Rating: {recipe.rating}
          </div>
        )}

        {/* Tags */}
        {recipe.tags.length > 0 && (
          <div className="flex flex-wrap gap-1">
            {recipe.tags.slice(0, 3).map((tag) => (
              <Badge key={tag} variant="secondary" className="text-xs">
                {tag}
              </Badge>
            ))}
            {recipe.tags.length > 3 && (
              <Badge variant="outline" className="text-xs">
                +{recipe.tags.length - 3}
              </Badge>
            )}
          </div>
        )}

        {/* Flags */}
        {recipe.flags.length > 0 && (
          <div className="mt-2 flex flex-wrap gap-1">
            {recipe.flags.slice(0, 2).map((flag) => (
              <Badge key={flag} variant="outline" className="text-xs">
                {flag}
              </Badge>
            ))}
            {recipe.flags.length > 2 && (
              <Badge variant="outline" className="text-xs">
                +{recipe.flags.length - 2}
              </Badge>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  )
}
