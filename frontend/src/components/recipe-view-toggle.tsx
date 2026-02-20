import { motion } from 'motion/react'
import {
  GridViewIcon,
  LayoutGridIcon,
  LeftToRightListBulletIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import { cn } from '@/lib/utils'

export type RecipeViewMode = 'cards' | 'small-cards' | 'list'

const viewOptions: {
  value: RecipeViewMode
  icon: typeof GridViewIcon
  label: string
}[] = [
  { value: 'cards', icon: GridViewIcon, label: 'Cards' },
  { value: 'small-cards', icon: LayoutGridIcon, label: 'Small cards' },
  { value: 'list', icon: LeftToRightListBulletIcon, label: 'List' },
]

interface RecipeViewToggleProps {
  value: RecipeViewMode
  onChange: (value: RecipeViewMode) => void
}

export function RecipeViewToggle({ value, onChange }: RecipeViewToggleProps) {
  return (
    <div className="inline-flex items-center rounded-lg border bg-muted p-0.5">
      {viewOptions.map((option) => (
        <button
          key={option.value}
          onClick={() => onChange(option.value)}
          className={cn(
            'relative inline-flex items-center justify-center rounded-md px-2 py-1 text-sm transition-colors',
            value === option.value
              ? 'text-foreground'
              : 'text-muted-foreground hover:text-foreground',
          )}
          aria-label={option.label}
        >
          {value === option.value && (
            <motion.span
              layoutId="recipe-view-indicator"
              className="absolute inset-0 rounded-md bg-background shadow-sm"
              transition={{ type: 'spring', bounce: 0.15, duration: 0.5 }}
            />
          )}
          <HugeiconsIcon icon={option.icon} className="relative z-10 size-4" />
        </button>
      ))}
    </div>
  )
}
