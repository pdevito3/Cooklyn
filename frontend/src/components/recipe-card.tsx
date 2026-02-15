import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type { RecipeSummaryDto } from "@/domain/recipes";
import {
  Delete01Icon,
  Edit01Icon,
  MoreVerticalIcon,
  ShoppingCart01Icon,
} from "@hugeicons/core-free-icons";
import { HugeiconsIcon } from "@hugeicons/react";
import { Link } from "@tanstack/react-router";
import { RatingIcon } from "@/components/rating-icon";

interface RecipeCardProps {
  recipe: RecipeSummaryDto;
  onEdit?: (id: string) => void;
  onDelete?: (id: string) => void;
  onAddToShoppingList?: (id: string) => void;
}

export function RecipeCard({
  recipe,
  onEdit,
  onDelete,
  onAddToShoppingList,
}: RecipeCardProps) {
  const placeholderImage =
    'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300"%3E%3Crect fill="%23374151" width="400" height="300"/%3E%3Ctext fill="%239ca3af" font-family="system-ui" font-size="20" x="50%25" y="50%25" text-anchor="middle" dominant-baseline="middle"%3ENo Image%3C/text%3E%3C/svg%3E';

  return (
    <div className="group relative aspect-[4/3] overflow-hidden rounded-xl shadow-md transition-all hover:shadow-xl">
      {/* Full card background image */}
      <Link
        to="/recipes/$id"
        params={{ id: recipe.id }}
        className="absolute inset-0"
      >
        <img
          src={recipe.imageUrl ?? placeholderImage}
          alt={recipe.title}
          className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
      </Link>

      {/* Gradient overlay for text readability */}
      <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent" />

      {/* Top right actions */}
      <div className="absolute right-2 top-2 z-10 flex items-center gap-1">
        {/* Rating indicator */}
        {recipe.rating && recipe.rating !== "Not Rated" && (
          <span className="flex h-8 w-8 items-center justify-center rounded-full bg-white/90 shadow-sm">
            <RatingIcon rating={recipe.rating} size="sm" />
          </span>
        )}

        {/* Context menu */}
        <DropdownMenu>
          <DropdownMenuTrigger
            render={
              <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 bg-white/90 shadow-sm hover:bg-white"
                onClick={(e) => e.stopPropagation()}
              >
                <HugeiconsIcon
                  icon={MoreVerticalIcon}
                  className="h-4 w-4 text-gray-700"
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
                <HugeiconsIcon icon={ShoppingCart01Icon} className="mr-2 h-4 w-4" />
                Add to Shopping List
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

      {/* Bottom content overlay */}
      <div className="absolute inset-x-0 bottom-0 z-10 p-4">
        <Link to="/recipes/$id" params={{ id: recipe.id }} className="block">
          <h3 className="text-xl font-bold leading-tight text-white drop-shadow-md transition-colors hover:text-white/90 sm:text-2xl">
            {recipe.title}
          </h3>
        </Link>

        {/* Optional description */}
        {recipe.description && (
          <p className="mt-1 line-clamp-1 text-sm text-white/80">
            {recipe.description}
          </p>
        )}
      </div>
    </div>
  );
}
