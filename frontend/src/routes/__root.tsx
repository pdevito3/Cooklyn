import { useState, useEffect } from 'react'
import {
  createRootRouteWithContext,
  Outlet,
  useRouterState,
} from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'
import type { QueryClient } from '@tanstack/react-query'
import { useHotkeys } from 'react-hotkeys-hook'
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
import { getUser } from '@/domain/auth/apis/get-user'
import { AuthKeys } from '@/domain/auth/apis/auth.keys'

export interface RouterContext {
  queryClient: QueryClient
}

export const Route = createRootRouteWithContext<RouterContext>()({
  beforeLoad: async ({ context, location }) => {
    try {
      await context.queryClient.ensureQueryData({
        queryKey: AuthKeys.user(),
        queryFn: getUser,
        staleTime: 5 * 60 * 1000,
      })
    } catch {
      window.location.href = `/bff/login?returnUrl=${encodeURIComponent(location.href)}`
      // Halt the router while the browser navigates to the IdP
      await new Promise(() => {})
    }
  },
  component: RootComponent,
})

const routeNames: Record<string, string> = {
  '/': 'Home',
  '/recipes': 'Recipes',
  '/recipes/new': 'New Recipe',
  '/meal-plan': 'Meal Plan',
  '/components': 'Components',
  '/filter-demo': 'Filter Builder',
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
  }, [isMobile])

  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarInset>
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
        <div className="flex flex-1 flex-col gap-4 p-4 pt-0">
          <Outlet />
        </div>
      </SidebarInset>
      <CommandMenu open={commandMenuOpen} onOpenChange={setCommandMenuOpen} />
      <TanStackRouterDevtools />
    </SidebarProvider>
  )
}
