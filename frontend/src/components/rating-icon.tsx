import { HugeiconsIcon } from '@hugeicons/react'
import {
  FavouriteIcon,
  ThumbsUpIcon,
  ThumbsDownIcon,
  NeutralIcon,
  Sad02Icon,
} from '@hugeicons/core-free-icons'
import { cn } from '@/lib/utils'

const sizeClasses = {
  sm: 'h-4 w-4',
  md: 'h-5 w-5',
  lg: 'h-6 w-6',
} as const

const ratingConfig: Record<
  string,
  { icon: typeof FavouriteIcon; colorClass: string; label: string }
> = {
  'Loved It': {
    icon: FavouriteIcon,
    colorClass: 'text-rose-500',
    label: 'Loved It',
  },
  'Liked It': {
    icon: ThumbsUpIcon,
    colorClass: 'text-emerald-500',
    label: 'Liked It',
  },
  'It Was Ok': {
    icon: NeutralIcon,
    colorClass: 'text-amber-500',
    label: 'It Was Ok',
  },
  'Not Great': {
    icon: ThumbsDownIcon,
    colorClass: 'text-orange-500',
    label: 'Not Great',
  },
  'Hated It': {
    icon: Sad02Icon,
    colorClass: 'text-red-700',
    label: 'Hated It',
  },
}

interface RatingIconProps {
  rating: string
  className?: string
  size?: 'sm' | 'md' | 'lg'
  showLabel?: boolean
}

export function RatingIcon({
  rating,
  className,
  size = 'md',
  showLabel = false,
}: RatingIconProps) {
  const config = ratingConfig[rating]
  if (!config) return null

  return (
    <span className={cn('inline-flex items-center gap-1', className)}>
      <HugeiconsIcon
        icon={config.icon}
        className={cn(sizeClasses[size], config.colorClass)}
        strokeWidth={2}
      />
      {showLabel && (
        <span className={cn('text-sm', config.colorClass)}>{config.label}</span>
      )}
    </span>
  )
}
