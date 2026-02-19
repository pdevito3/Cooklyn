import { useNavigate } from '@tanstack/react-router'
import { useDraggable } from '@dnd-kit/core'
import { CSS } from '@dnd-kit/utilities'
import type { MealPlanEntryDto } from '@/domain/meal-plans/types'
import type { MealPlanDensity } from '@/components/meal-plan/meal-plan-view-toggle'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { Button } from '@/components/ui/button'
import {
  MoreVerticalIcon,
  Edit01Icon,
  Copy01Icon,
  Delete01Icon,
  RestaurantIcon,
  StickyNote01Icon,
  Image01Icon,
  ArrowUpRight01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { cn } from '@/lib/utils'

interface MealPlanEntryCardProps {
  entry: MealPlanEntryDto
  onEdit: (entry: MealPlanEntryDto) => void
  onCopy: (entry: MealPlanEntryDto) => void
  onDelete: (id: string) => void
  compact?: boolean
  density?: MealPlanDensity
}

export function MealPlanEntryCard({
  entry,
  onEdit,
  onCopy,
  onDelete,
  compact = false,
  density = 'condensed',
}: MealPlanEntryCardProps) {
  const navigate = useNavigate()

  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({
      id: `entry-${entry.id}`,
      data: { type: 'entry', entry },
    })

  const style = {
    transform: CSS.Translate.toString(transform),
    opacity: isDragging ? 0.5 : 1,
  }

  const isFreeText = entry.entryType === 'FreeText'
  const isNormalRecipe = !compact && density === 'normal' && !isFreeText

  if (compact) {
    return (
      <div
        ref={setNodeRef}
        style={style}
        {...listeners}
        {...attributes}
        className={cn(
          'group flex items-center gap-1 rounded px-1.5 py-0.5 text-xs cursor-grab active:cursor-grabbing',
          isFreeText
            ? 'bg-emerald-50 hover:bg-emerald-100 dark:bg-emerald-950/30 dark:hover:bg-emerald-950/50'
            : 'hover:bg-accent',
        )}
      >
        <HugeiconsIcon
          icon={isFreeText ? StickyNote01Icon : RestaurantIcon}
          className="size-3 shrink-0 text-muted-foreground"
        />
        <span className="truncate">{entry.title}</span>
        {!isFreeText && entry.scale !== 1 && (
          <span className="text-muted-foreground shrink-0">
            {entry.scale}x
          </span>
        )}
      </div>
    )
  }

  if (isNormalRecipe) {
    return (
      <div
        ref={setNodeRef}
        style={style}
        {...listeners}
        {...attributes}
        className="group rounded-md border bg-card shadow-sm hover:shadow cursor-grab active:cursor-grabbing overflow-hidden"
      >
        {entry.imageUrl ? (
          <img
            src={entry.imageUrl}
            alt=""
            className="h-24 w-full object-cover"
          />
        ) : (
          <div className="flex h-16 w-full items-center justify-center bg-muted">
            <HugeiconsIcon icon={Image01Icon} className="size-6 text-muted-foreground" />
          </div>
        )}
        <div className="flex items-start gap-1 p-2">
          <div className="flex-1 min-w-0">
            <span className="text-sm font-medium leading-tight line-clamp-2">
              {entry.title}
            </span>
            {entry.scale !== 1 && (
              <span className="text-xs text-muted-foreground">
                {entry.scale}x
              </span>
            )}
          </div>
          <DropdownMenu>
            <DropdownMenuTrigger
              render={
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6 shrink-0 opacity-0 group-hover:opacity-100"
                  onPointerDown={(e) => e.stopPropagation()}
                />
              }
            >
              <HugeiconsIcon icon={MoreVerticalIcon} className="size-3.5" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-36">
              {entry.recipeId && (
                <DropdownMenuItem
                  onClick={() =>
                    navigate({
                      to: '/recipes/$id/',
                      params: { id: entry.recipeId! },
                    })
                  }
                >
                  <HugeiconsIcon icon={ArrowUpRight01Icon} className="size-4" />
                  View Recipe
                </DropdownMenuItem>
              )}
              <DropdownMenuItem onClick={() => onEdit(entry)}>
                <HugeiconsIcon icon={Edit01Icon} className="size-4" />
                Edit
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => onCopy(entry)}>
                <HugeiconsIcon icon={Copy01Icon} className="size-4" />
                Copy
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => onDelete(entry.id)}
              >
                <HugeiconsIcon icon={Delete01Icon} className="size-4" />
                Delete
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    )
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
      className={cn(
        'group flex items-center gap-1.5 rounded-md border shadow-sm hover:shadow cursor-grab active:cursor-grabbing',
        isFreeText
          ? 'bg-emerald-50 border-emerald-200 dark:bg-emerald-950/30 dark:border-emerald-900/50 px-2 py-1.5 text-sm'
          : 'bg-card px-2 py-1.5 text-sm',
      )}
    >
      <HugeiconsIcon
        icon={isFreeText ? StickyNote01Icon : RestaurantIcon}
        className="size-3.5 shrink-0 text-muted-foreground"
      />
      <span className="flex-1 truncate">{entry.title}</span>
      {!isFreeText && entry.scale !== 1 && (
        <span className="text-xs text-muted-foreground shrink-0">
          {entry.scale}x
        </span>
      )}
      <DropdownMenu>
        <DropdownMenuTrigger
          render={
            <Button
              variant="ghost"
              size="icon"
              className="h-5 w-5 shrink-0 opacity-0 group-hover:opacity-100"
              onPointerDown={(e) => e.stopPropagation()}
            />
          }
        >
          <HugeiconsIcon icon={MoreVerticalIcon} className="size-3" />
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-36">
          {entry.recipeId && (
            <DropdownMenuItem
              onClick={() =>
                navigate({
                  to: '/recipes/$id/',
                  params: { id: entry.recipeId! },
                })
              }
            >
              <HugeiconsIcon icon={ArrowUpRight01Icon} className="size-4" />
              View Recipe
            </DropdownMenuItem>
          )}
          <DropdownMenuItem onClick={() => onEdit(entry)}>
            <HugeiconsIcon icon={Edit01Icon} className="size-4" />
            Edit
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => onCopy(entry)}>
            <HugeiconsIcon icon={Copy01Icon} className="size-4" />
            Copy
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            variant="destructive"
            onClick={() => onDelete(entry.id)}
          >
            <HugeiconsIcon icon={Delete01Icon} className="size-4" />
            Delete
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  )
}
