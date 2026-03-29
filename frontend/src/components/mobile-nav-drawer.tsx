import { Link, useRouterState } from '@tanstack/react-router'
import {
  DashboardSquare01Icon,
  ArrowRight01Icon,
  FileImportIcon,
  Add01Icon,
  Settings01Icon,
  Search01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  Drawer,
  DrawerContent,
  DrawerFooter,
  DrawerHeader,
  DrawerTitle,
  DrawerDescription,
} from '@/components/ui/drawer'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { Separator } from '@/components/ui/separator'
import { ThemeToggle } from '@/components/theme-toggle'
import { cn } from '@/lib/utils'
import {
  navItems,
  settingsItems,
  importItems,
  demoItems,
} from '@/components/app-sidebar'

interface MobileNavDrawerProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

const collapsibleSections = [
  {
    title: 'Import',
    icon: FileImportIcon,
    items: importItems,
  },
  {
    title: 'Settings',
    icon: Settings01Icon,
    items: settingsItems,
  },
  {
    title: 'Demos',
    icon: DashboardSquare01Icon,
    items: demoItems,
  },
]

export function MobileNavDrawer({ open, onOpenChange }: MobileNavDrawerProps) {
  const routerState = useRouterState()
  const currentPath = routerState.location.pathname

  function handleNav() {
    onOpenChange(false)
  }

  function handleAction(eventName: string) {
    onOpenChange(false)
    window.dispatchEvent(new CustomEvent(eventName))
  }

  return (
    <Drawer open={open} onOpenChange={onOpenChange}>
      <DrawerContent className="max-h-[70vh]">
        <DrawerHeader className="sr-only">
          <DrawerTitle>Navigation</DrawerTitle>
          <DrawerDescription>App navigation menu</DrawerDescription>
        </DrawerHeader>

        <nav className="flex-1 overflow-y-auto px-4 pb-2">
          {/* Primary nav items */}
          <div className="flex flex-col gap-1">
            {navItems.map((item) => {
              const isActive = currentPath === item.url
              return (
                <Link
                  key={item.url}
                  to={item.url}
                  onClick={handleNav}
                  className={cn(
                    'flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-accent text-accent-foreground'
                      : 'text-muted-foreground active:bg-accent/50',
                  )}
                >
                  <HugeiconsIcon icon={item.icon} className="size-5" />
                  <span>{item.title}</span>
                </Link>
              )
            })}
          </div>

          <Separator className="my-2" />

          {/* Search & Quick Add */}
          <div className="flex flex-col gap-1">
            <button
              type="button"
              onClick={() => handleAction('open-command-menu')}
              className="flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium text-muted-foreground active:bg-accent/50 transition-colors"
            >
              <HugeiconsIcon icon={Search01Icon} className="size-5" />
              <span>Search</span>
            </button>
            <button
              type="button"
              onClick={() => handleAction('open-quick-add')}
              className="flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium text-muted-foreground active:bg-accent/50 transition-colors"
            >
              <HugeiconsIcon icon={Add01Icon} className="size-5" />
              <span>Add Item</span>
            </button>
          </div>

          <Separator className="my-2" />

          {/* Collapsible sections */}
          <div className="flex flex-col gap-1">
            {collapsibleSections.map((section) => {
              const isSectionActive = section.items.some(
                (item) =>
                  currentPath === item.url ||
                  currentPath.startsWith(item.url + '/'),
              )

              return (
                <Collapsible
                  key={section.title}
                  defaultOpen={isSectionActive}
                  className="group/collapsible"
                >
                  <CollapsibleTrigger className="flex w-full items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium text-muted-foreground active:bg-accent/50 transition-colors">
                    <HugeiconsIcon icon={section.icon} className="size-5" />
                    <span>{section.title}</span>
                    <HugeiconsIcon
                      icon={ArrowRight01Icon}
                      className="ml-auto size-4 transition-transform duration-200 group-data-[open]/collapsible:rotate-90"
                    />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <div className="ml-8 flex flex-col gap-1">
                      {section.items.map((item) => {
                        const isActive =
                          currentPath === item.url ||
                          currentPath.startsWith(item.url + '/')
                        return (
                          <Link
                            key={item.url}
                            to={item.url}
                            onClick={handleNav}
                            className={cn(
                              'flex items-center rounded-lg px-4 py-2.5 text-sm transition-colors',
                              isActive
                                ? 'bg-accent text-accent-foreground font-medium'
                                : 'text-muted-foreground active:bg-accent/50',
                            )}
                          >
                            {item.title}
                          </Link>
                        )
                      })}
                    </div>
                  </CollapsibleContent>
                </Collapsible>
              )
            })}
          </div>
        </nav>

        <DrawerFooter>
          <div className="flex items-center justify-center">
            <ThemeToggle />
          </div>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  )
}
