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
  ArrowUpRight01Icon,
  DragDropIcon,
  ShoppingCart01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { cn } from '@/lib/utils'

interface MealPlanEntryCardProps {
  entry: MealPlanEntryDto
  onEdit: (entry: MealPlanEntryDto) => void
  onCopy: (entry: MealPlanEntryDto) => void
  onDelete: (id: string) => void
  onAddToShoppingList?: (recipeId: string) => void
  compact?: boolean
  density?: MealPlanDensity
}

export function MealPlanEntryCard({
  entry,
  onEdit,
  onCopy,
  onDelete,
  onAddToShoppingList,
  compact = false,
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

  if (compact) {
    return (
      <div
        ref={setNodeRef}
        style={style}
        {...listeners}
        {...attributes}
        className={cn(
          'group flex items-center gap-1 rounded text-xs cursor-grab active:cursor-grabbing',
          isFreeText
            ? 'bg-emerald-50 hover:bg-emerald-100 dark:bg-emerald-900/40 dark:hover:bg-emerald-900/60 dark:text-emerald-50 px-1.5 py-2'
            : 'hover:bg-accent px-1.5 py-0.5',
        )}
      >
        <HugeiconsIcon
          icon={isFreeText ? StickyNote01Icon : RestaurantIcon}
          className="size-3 shrink-0 text-muted-foreground"
        />
        <span className={isFreeText ? 'whitespace-pre-line' : 'truncate'}>{entry.title}</span>
        {!isFreeText && entry.scale !== 1 && (
          <span className="text-muted-foreground shrink-0">
            {entry.scale}x
          </span>
        )}
      </div>
    )
  }

  const entryMenu = (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={
          <Button
            variant="ghost"
            size="icon"
            className="h-7 w-7 shrink-0 md:opacity-0 md:group-hover:opacity-100"
          />
        }
      >
        <HugeiconsIcon icon={MoreVerticalIcon} className="size-4" />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-44">
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
        {entry.recipeId && onAddToShoppingList && (
          <DropdownMenuItem
            onClick={() => onAddToShoppingList(entry.recipeId!)}
          >
            <HugeiconsIcon icon={ShoppingCart01Icon} className="size-4" />
            Add to Shopping List
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
  )

  if (isFreeText) {
    return (
      <div
        ref={setNodeRef}
        style={style}
        className="group flex items-start gap-1.5 rounded-md border bg-emerald-50 border-emerald-200 dark:bg-emerald-900/40 dark:border-emerald-700/50 dark:text-emerald-50 px-2 py-2 text-sm shadow-sm hover:shadow"
      >
        <button
          type="button"
          aria-label="Drag to move"
          className="inline-flex h-7 w-7 shrink-0 items-center justify-center rounded-md text-muted-foreground hover:text-foreground cursor-grab active:cursor-grabbing touch-none -ml-1"
          {...listeners}
          {...attributes}
        >
          <HugeiconsIcon icon={DragDropIcon} className="size-4" />
        </button>
        <HugeiconsIcon
          icon={StickyNote01Icon}
          className="size-4 shrink-0 mt-1 text-muted-foreground"
        />
        <span className="flex-1 min-w-0 whitespace-pre-line break-words pt-0.5">
          {entry.title}
        </span>
        <div className="-mr-1 mt-[-2px]">{entryMenu}</div>
      </div>
    )
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      className="group relative rounded-md border bg-card shadow-sm hover:shadow overflow-hidden"
    >
      <button
        type="button"
        aria-label="Drag to move"
        className={cn(
          'inline-flex h-7 w-7 items-center justify-center rounded-md text-muted-foreground hover:text-foreground cursor-grab active:cursor-grabbing touch-none shadow-sm',
          entry.imageUrl
            ? 'absolute top-1 left-1 z-10 bg-background/80 backdrop-blur-sm'
            : 'absolute top-1 left-1 z-10',
        )}
        {...listeners}
        {...attributes}
      >
        <HugeiconsIcon icon={DragDropIcon} className="size-4" />
      </button>
      {entry.imageUrl && (
        <img
          src={entry.imageUrl}
          alt=""
          className="aspect-[4/3] w-full object-cover"
        />
      )}
      <div className={cn('flex items-start gap-1 p-2', !entry.imageUrl && 'pl-10')}>
        <div className="flex-1 min-w-0">
          <span className="text-sm font-medium leading-tight break-words">
            {entry.title}
          </span>
          {entry.scale !== 1 && (
            <span className="ml-1 text-xs text-muted-foreground">
              {entry.scale}x
            </span>
          )}
        </div>
        {entryMenu}
      </div>
    </div>
  )
}
