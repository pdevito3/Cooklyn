import {
  Drawer,
  DrawerContent,
  DrawerDescription,
  DrawerHeader,
  DrawerTitle,
} from '@/components/ui/drawer'
import {
  FilterIcon,
  LinkSquare02Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

interface RecipeSourceActionSheetProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  source: string
  domain: string
  onFilterByDomain: () => void
}

export function RecipeSourceActionSheet({
  open,
  onOpenChange,
  source,
  domain,
  onFilterByDomain,
}: RecipeSourceActionSheetProps) {
  const handleOpen = () => {
    window.open(source, '_blank', 'noopener,noreferrer')
    onOpenChange(false)
  }

  const handleFilter = () => {
    onFilterByDomain()
    onOpenChange(false)
  }

  return (
    <Drawer open={open} onOpenChange={onOpenChange}>
      <DrawerContent>
        <DrawerHeader>
          <DrawerTitle>{domain}</DrawerTitle>
          <DrawerDescription className="truncate">{source}</DrawerDescription>
        </DrawerHeader>
        <div className="flex flex-col gap-1 p-2 pb-6">
          <button
            type="button"
            onClick={handleOpen}
            className="flex w-full items-center gap-3 rounded-md px-3 py-3 text-left text-sm hover:bg-accent"
          >
            <HugeiconsIcon icon={LinkSquare02Icon} className="h-4 w-4" />
            Open link
          </button>
          <button
            type="button"
            onClick={handleFilter}
            className="flex w-full items-center gap-3 rounded-md px-3 py-3 text-left text-sm hover:bg-accent"
          >
            <HugeiconsIcon icon={FilterIcon} className="h-4 w-4" />
            Filter by {domain}
          </button>
        </div>
      </DrawerContent>
    </Drawer>
  )
}
