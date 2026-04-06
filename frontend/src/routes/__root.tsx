import { useState, useEffect } from 'react'
import {
  createRootRouteWithContext,
  Outlet,
  useRouterState,
} from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'
import type { QueryClient } from '@tanstack/react-query'
import { useHotkeys } from 'react-hotkeys-hook'
import { Menu01Icon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import {
  SidebarInset,
  SidebarProvider,
  SidebarTrigger,
} from '@/components/ui/sidebar'
import { useIsMobile } from '@/hooks/use-mobile'
import { AppSidebar } from '@/components/app-sidebar'
import { Separator } from '@/components/ui/separator'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbList,
  BreadcrumbPage,
} from '@/components/ui/breadcrumb'
import { CommandMenu } from '@/components/command-menu'
import { MobileNavDrawer } from '@/components/mobile-nav-drawer'

export interface RouterContext {
  queryClient: QueryClient
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
})

const routeNames: Record<string, string> = {
  '/': 'Home',
  '/recipes': 'Recipes',
  '/recipes/new': 'New Recipe',
  '/meal-plan': 'Meal Plan',
  '/components': 'Components',
  '/filter-demo': 'Filter Builder',
  '/build': 'Build Info',
}

// Dynamic route name resolver for parameterized routes
function getPageName(path: string): string {
  // Check static routes first
  if (routeNames[path]) return routeNames[path]

  // Handle /recipes/$id pattern
  if (/^\/recipes\/[^/]+$/.test(path)) return 'Recipe Details'

  // Handle /recipes/$id/edit pattern
  if (/^\/recipes\/[^/]+\/edit$/.test(path)) return 'Edit Recipe'

  return 'Page'
}

function RootComponent() {
  const routerState = useRouterState()
  const currentPath = routerState.location.pathname
  const pageName = getPageName(currentPath)
  const isMobile = useIsMobile()
  const [commandMenuOpen, setCommandMenuOpen] = useState(false)
  const [mobileNavOpen, setMobileNavOpen] = useState(false)

  useHotkeys(
    'mod+k',
    (e) => {
      e.preventDefault()
      setCommandMenuOpen((prev) => !prev)
    },
    { enableOnFormTags: ['INPUT', 'TEXTAREA', 'SELECT'] },
  )

  useEffect(() => {
    function handleOpenCommandMenu() {
      setCommandMenuOpen(true)
    }
    window.addEventListener('open-command-menu', handleOpenCommandMenu)
    return () =>
      window.removeEventListener('open-command-menu', handleOpenCommandMenu)
  }, [])

  // Mobile: swipe-up from bottom edge opens command menu
  useEffect(() => {
    if (!isMobile) return

    let startY = 0
    let isEdgeSwipe = false

    function handleTouchStart(e: TouchEvent) {
      const touch = e.touches[0]
      if (touch.clientY >= window.innerHeight - 30) {
        startY = touch.clientY
        isEdgeSwipe = true
      } else {
        isEdgeSwipe = false
      }
    }

    function handleTouchEnd(e: TouchEvent) {
      if (!isEdgeSwipe) return
      if (mobileNavOpen) return
      const touch = e.changedTouches[0]
      const deltaY = startY - touch.clientY
      if (deltaY >= 60) {
        setCommandMenuOpen(true)
      }
      isEdgeSwipe = false
    }

    document.addEventListener('touchstart', handleTouchStart, { passive: true })
    document.addEventListener('touchend', handleTouchEnd, { passive: true })
    return () => {
      document.removeEventListener('touchstart', handleTouchStart)
      document.removeEventListener('touchend', handleTouchEnd)
    }
  }, [isMobile, mobileNavOpen])

  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarInset>
        {!isMobile && (
          <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
            <div className="flex items-center gap-2 px-4">
              <SidebarTrigger className="-ml-1" />
              <Separator
                orientation="vertical"
                className="mr-2 data-[orientation=vertical]:h-4"
              />
              <Breadcrumb>
                <BreadcrumbList>
                  <BreadcrumbItem>
                    <BreadcrumbPage>{pageName}</BreadcrumbPage>
                  </BreadcrumbItem>
                </BreadcrumbList>
              </Breadcrumb>
            </div>
          </header>
        )}
        <div className="flex flex-1 flex-col gap-4 p-4 pt-0 max-md:pt-2">
          <Outlet />
        </div>
      </SidebarInset>
      <CommandMenu open={commandMenuOpen} onOpenChange={setCommandMenuOpen} />
      {isMobile && (
        <>
          <MobileNavDrawer
            open={mobileNavOpen}
            onOpenChange={setMobileNavOpen}
          />
          {!mobileNavOpen && (
            <button
              type="button"
              onClick={() => setMobileNavOpen(true)}
              className="fixed bottom-6 right-6 z-40 flex size-14 items-center justify-center rounded-full bg-primary text-primary-foreground shadow-lg active:scale-95 transition-transform"
              aria-label="Open navigation menu"
            >
              <HugeiconsIcon icon={Menu01Icon} className="size-6" />
            </button>
          )}
        </>
      )}
      {!isMobile && <TanStackRouterDevtools position="bottom-right" />}
    </SidebarProvider>
  )
}
