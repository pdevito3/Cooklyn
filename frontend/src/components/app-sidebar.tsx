import { Link, useNavigate, useRouterState } from '@tanstack/react-router'
import { useState, useEffect, useRef } from 'react'
import {
  Home01Icon,
  DashboardSquare01Icon,
  ArrowRight01Icon,
  RestaurantIcon,
  Calendar03Icon,
  FileImportIcon,
  ShoppingCart01Icon,
  Add01Icon,
  Settings01Icon,
  Search01Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
  SidebarRail,
} from '@/components/ui/sidebar'
import { ThemeToggle } from '@/components/theme-toggle'
import { Kbd } from '@/components/ui/kbd'
import { QuickAddDialog } from '@/components/quick-add-dialog'

export const navItems = [
  {
    title: 'Home',
    url: '/',
    icon: Home01Icon,
    hotkey: 'H',
  },
  {
    title: 'Shopping Lists',
    url: '/shopping-lists',
    icon: ShoppingCart01Icon,
    hotkey: 'L',
  },
  {
    title: 'Recipes',
    url: '/recipes',
    icon: RestaurantIcon,
    hotkey: 'R',
  },
  {
    title: 'Meal Plan',
    url: '/meal-plan',
    icon: Calendar03Icon,
    hotkey: 'P',
  },
]

export const settingsItems = [
  {
    title: 'Stores',
    url: '/stores',
    hotkey: 'S',
  },
  {
    title: 'Collections',
    url: '/collections',
    hotkey: 'C',
  },
]

export const importItems = [
  {
    title: 'From URL',
    url: '/recipes/import',
    hotkey: 'I',
  },
  {
    title: 'From Copy Me That',
    url: '/recipes/import-cmt',
    hotkey: 'M',
  },
]

export const demoItems = [
  {
    title: 'Components',
    url: '/components',
  },
  {
    title: 'Filter Builder',
    url: '/filter-demo',
  },
]

const gHotkeys: Record<string, string> = {}
for (const item of navItems) gHotkeys[item.hotkey.toLowerCase()] = item.url
for (const item of settingsItems) gHotkeys[item.hotkey.toLowerCase()] = item.url
for (const item of importItems) gHotkeys[item.hotkey.toLowerCase()] = item.url

export function AppSidebar() {
  const routerState = useRouterState()
  const currentPath = routerState.location.pathname
  const navigate = useNavigate()
  const gPressedRef = useRef(false)
  const [quickAddOpen, setQuickAddOpen] = useState(false)

  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      const target = e.target as HTMLElement
      if (
        target.tagName === 'INPUT' ||
        target.tagName === 'TEXTAREA' ||
        target.tagName === 'SELECT' ||
        target.isContentEditable
      ) {
        return
      }

      if (e.key === 'g' && !e.metaKey && !e.ctrlKey && !e.altKey) {
        gPressedRef.current = true
        setTimeout(() => {
          gPressedRef.current = false
        }, 1000)
        return
      }

      if (gPressedRef.current) {
        gPressedRef.current = false
        if (e.key === 'n') {
          e.preventDefault()
          setQuickAddOpen(true)
          return
        }
        const url = gHotkeys[e.key]
        if (url) {
          e.preventDefault()
          navigate({ to: url })
        }
      }
    }

    function handleOpenQuickAdd() {
      setQuickAddOpen(true)
    }

    window.addEventListener('keydown', handleKeyDown)
    window.addEventListener('open-quick-add', handleOpenQuickAdd)
    return () => {
      window.removeEventListener('keydown', handleKeyDown)
      window.removeEventListener('open-quick-add', handleOpenQuickAdd)
    }
  }, [navigate])

  // Check if any import item is active
  const isImportActive = importItems.some(
    (item) =>
      currentPath === item.url || currentPath.startsWith(item.url + '/'),
  )

  // Check if any settings item is active
  const isSettingsActive = settingsItems.some(
    (item) =>
      currentPath === item.url || currentPath.startsWith(item.url + '/'),
  )

  // Check if any demo item is active
  const isDemoActive = demoItems.some(
    (item) =>
      currentPath === item.url || currentPath.startsWith(item.url + '/'),
  )

  return (
    <Sidebar variant="inset" collapsible="icon">
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" render={<Link to="/" />}>
              <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                <span className="text-xs font-bold">FS</span>
              </div>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-semibold">Fullstack</span>
                <span className="truncate text-xs text-muted-foreground">
                  Template
                </span>
              </div>
            </SidebarMenuButton>
            <button
              type="button"
              onClick={() =>
                window.dispatchEvent(new CustomEvent('open-command-menu'))
              }
              className="text-muted-foreground hover:text-foreground hover:bg-sidebar-accent absolute right-2 top-1/2 -translate-y-1/2 flex size-6 items-center justify-center rounded-md transition-colors group-data-[collapsible=icon]:hidden"
            >
              <HugeiconsIcon icon={Search01Icon} className="size-4" />
            </button>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Navigation</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {/* Search */}
              <SidebarMenuItem>
                <SidebarMenuButton
                  onClick={() =>
                    window.dispatchEvent(new CustomEvent('open-command-menu'))
                  }
                >
                  <HugeiconsIcon icon={Search01Icon} />
                  <span>Search</span>
                  <Kbd>⌘K</Kbd>
                </SidebarMenuButton>
              </SidebarMenuItem>

              {navItems.map((item) => {
                const isActive = currentPath === item.url
                return (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton
                      render={<Link to={item.url} />}
                      isActive={isActive}
                    >
                      <HugeiconsIcon icon={item.icon} />
                      <span>{item.title}</span>
                      <Kbd>G+{item.hotkey}</Kbd>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                )
              })}

              {/* Quick Add */}
              <SidebarMenuItem>
                <SidebarMenuButton onClick={() => setQuickAddOpen(true)}>
                  <HugeiconsIcon icon={Add01Icon} />
                  <span>Add Item</span>
                  <Kbd>G+N</Kbd>
                </SidebarMenuButton>
              </SidebarMenuItem>

              {/* Import submenu */}
              <Collapsible
                defaultOpen={isImportActive}
                className="group/collapsible"
              >
                <SidebarMenuItem>
                  <CollapsibleTrigger
                    className="ring-sidebar-ring hover:bg-sidebar-accent hover:text-sidebar-accent-foreground active:bg-sidebar-accent active:text-sidebar-accent-foreground data-[state=open]:hover:bg-sidebar-accent data-[state=open]:hover:text-sidebar-accent-foreground gap-2 rounded-md p-2 text-left text-sm transition-[width,height,padding] group-has-data-[sidebar=menu-action]/menu-item:pr-8 group-data-[collapsible=icon]:size-8! group-data-[collapsible=icon]:p-2! focus-visible:ring-2 peer/menu-button flex w-full items-center overflow-hidden outline-hidden [&>span:last-child]:truncate [&_svg]:size-4 [&_svg]:shrink-0 h-8 data-[active]:bg-sidebar-accent data-[active]:text-sidebar-accent-foreground data-[active]:font-medium"
                    data-active={isImportActive || undefined}
                  >
                    <HugeiconsIcon icon={FileImportIcon} />
                    <span>Import</span>
                    <HugeiconsIcon
                      icon={ArrowRight01Icon}
                      className="ml-auto transition-transform duration-200 group-data-[open]/collapsible:rotate-90"
                    />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {importItems.map((item) => {
                        const isActive = currentPath === item.url
                        return (
                          <SidebarMenuSubItem
                            key={item.title}
                            isActive={isActive}
                          >
                            <SidebarMenuSubButton
                              render={<Link to={item.url} />}
                              isActive={isActive}
                            >
                              <span>{item.title}</span>
                              <Kbd>G+{item.hotkey}</Kbd>
                            </SidebarMenuSubButton>
                          </SidebarMenuSubItem>
                        )
                      })}
                    </SidebarMenuSub>
                  </CollapsibleContent>
                </SidebarMenuItem>
              </Collapsible>

              {/* Settings submenu */}
              <Collapsible
                defaultOpen={isSettingsActive}
                className="group/collapsible"
              >
                <SidebarMenuItem>
                  <CollapsibleTrigger
                    className="ring-sidebar-ring hover:bg-sidebar-accent hover:text-sidebar-accent-foreground active:bg-sidebar-accent active:text-sidebar-accent-foreground data-[state=open]:hover:bg-sidebar-accent data-[state=open]:hover:text-sidebar-accent-foreground gap-2 rounded-md p-2 text-left text-sm transition-[width,height,padding] group-has-data-[sidebar=menu-action]/menu-item:pr-8 group-data-[collapsible=icon]:size-8! group-data-[collapsible=icon]:p-2! focus-visible:ring-2 peer/menu-button flex w-full items-center overflow-hidden outline-hidden [&>span:last-child]:truncate [&_svg]:size-4 [&_svg]:shrink-0 h-8 data-[active]:bg-sidebar-accent data-[active]:text-sidebar-accent-foreground data-[active]:font-medium"
                    data-active={isSettingsActive || undefined}
                  >
                    <HugeiconsIcon icon={Settings01Icon} />
                    <span>Settings</span>
                    <HugeiconsIcon
                      icon={ArrowRight01Icon}
                      className="ml-auto transition-transform duration-200 group-data-[open]/collapsible:rotate-90"
                    />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {settingsItems.map((item) => {
                        const isActive =
                          currentPath === item.url ||
                          currentPath.startsWith(item.url + '/')
                        return (
                          <SidebarMenuSubItem
                            key={item.title}
                            isActive={isActive}
                          >
                            <SidebarMenuSubButton
                              render={<Link to={item.url} />}
                              isActive={isActive}
                            >
                              <span>{item.title}</span>
                              <Kbd>G+{item.hotkey}</Kbd>
                            </SidebarMenuSubButton>
                          </SidebarMenuSubItem>
                        )
                      })}
                    </SidebarMenuSub>
                  </CollapsibleContent>
                </SidebarMenuItem>
              </Collapsible>

              {/* Demos submenu */}
              <Collapsible
                defaultOpen={isDemoActive}
                className="group/collapsible"
              >
                <SidebarMenuItem>
                  <CollapsibleTrigger
                    className="ring-sidebar-ring hover:bg-sidebar-accent hover:text-sidebar-accent-foreground active:bg-sidebar-accent active:text-sidebar-accent-foreground data-[state=open]:hover:bg-sidebar-accent data-[state=open]:hover:text-sidebar-accent-foreground gap-2 rounded-md p-2 text-left text-sm transition-[width,height,padding] group-has-data-[sidebar=menu-action]/menu-item:pr-8 group-data-[collapsible=icon]:size-8! group-data-[collapsible=icon]:p-2! focus-visible:ring-2 peer/menu-button flex w-full items-center overflow-hidden outline-hidden [&>span:last-child]:truncate [&_svg]:size-4 [&_svg]:shrink-0 h-8 data-[active]:bg-sidebar-accent data-[active]:text-sidebar-accent-foreground data-[active]:font-medium"
                    data-active={isDemoActive || undefined}
                  >
                    <HugeiconsIcon icon={DashboardSquare01Icon} />
                    <span>Demos</span>
                    <HugeiconsIcon
                      icon={ArrowRight01Icon}
                      className="ml-auto transition-transform duration-200 group-data-[open]/collapsible:rotate-90"
                    />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <SidebarMenuSub>
                      {demoItems.map((item) => {
                        const isActive = currentPath === item.url
                        return (
                          <SidebarMenuSubItem
                            key={item.title}
                            isActive={isActive}
                          >
                            <SidebarMenuSubButton
                              render={<Link to={item.url} />}
                              isActive={isActive}
                            >
                              <span>{item.title}</span>
                            </SidebarMenuSubButton>
                          </SidebarMenuSubItem>
                        )
                      })}
                    </SidebarMenuSub>
                  </CollapsibleContent>
                </SidebarMenuItem>
              </Collapsible>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <div className="flex items-center justify-center p-2 group-data-[collapsible=icon]:p-0">
              <ThemeToggle />
            </div>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
      <SidebarRail />
      <QuickAddDialog open={quickAddOpen} onOpenChange={setQuickAddOpen} />
    </Sidebar>
  )
}
