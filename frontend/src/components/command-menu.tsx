import { useState, useCallback } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useQuery } from '@tanstack/react-query'
import {
  RestaurantIcon,
  ShoppingCart01Icon,
  Store01Icon,
  Layers01Icon,
  FileImportIcon,
  Add01Icon,
  Loading03Icon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import {
  Command,
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandShortcut,
} from '@/components/ui/command'
import {
  Drawer,
  DrawerContent,
  DrawerTitle,
  DrawerDescription,
} from '@/components/ui/drawer'
import { useIsMobile } from '@/hooks/use-mobile'
import { useDebouncedValue } from '@/hooks/use-debounced-value'
import { getRecipes } from '@/domain/recipes/apis/get-recipes'
import { getShoppingLists } from '@/domain/shopping-lists/apis/get-shopping-lists'
import { getStores } from '@/domain/stores/apis/get-stores'
import { getItemCollections } from '@/domain/item-collections/apis/get-item-collections'
import { Badge } from '@/components/ui/badge'

interface CommandMenuProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function CommandMenu({ open, onOpenChange }: CommandMenuProps) {
  const navigate = useNavigate()
  const isMobile = useIsMobile()
  const [query, setQuery] = useState('')
  const debouncedQuery = useDebouncedValue(query, 250)

  const isSearching = debouncedQuery.length >= 2
  const searchFilter = (field: string) =>
    `${field} @=* "${debouncedQuery}"`

  const recipes = useQuery({
    queryKey: ['command-search', debouncedQuery, 'recipes'],
    queryFn: () =>
      getRecipes({ filters: searchFilter('title'), pageSize: 5 }),
    enabled: isSearching,
    staleTime: 30_000,
  })

  const shoppingLists = useQuery({
    queryKey: ['command-search', debouncedQuery, 'shopping-lists'],
    queryFn: () =>
      getShoppingLists({ filters: searchFilter('name'), pageSize: 5 }),
    enabled: isSearching,
    staleTime: 30_000,
  })

  const stores = useQuery({
    queryKey: ['command-search', debouncedQuery, 'stores'],
    queryFn: () =>
      getStores({ filters: searchFilter('name'), pageSize: 5 }),
    enabled: isSearching,
    staleTime: 30_000,
  })

  const collections = useQuery({
    queryKey: ['command-search', debouncedQuery, 'collections'],
    queryFn: () =>
      getItemCollections({ filters: searchFilter('name'), pageSize: 5 }),
    enabled: isSearching,
    staleTime: 30_000,
  })

  const isFetching =
    recipes.isFetching ||
    shoppingLists.isFetching ||
    stores.isFetching ||
    collections.isFetching

  const hasResults =
    (recipes.data?.items.length ?? 0) > 0 ||
    (shoppingLists.data?.items.length ?? 0) > 0 ||
    (stores.data?.items.length ?? 0) > 0 ||
    (collections.data?.items.length ?? 0) > 0

  const handleOpenChange = useCallback(
    (value: boolean) => {
      onOpenChange(value)
      if (!value) setQuery('')
    },
    [onOpenChange],
  )

  const handleSelect = useCallback(
    (value: string) => {
      handleOpenChange(false)

      if (value.startsWith('nav:')) {
        navigate({ to: value.replace('nav:', '') })
        return
      }

      if (value.startsWith('recipe:')) {
        navigate({
          to: '/recipes/$id',
          params: { id: value.replace('recipe:', '') },
        })
        return
      }

      if (value.startsWith('shopping-list:')) {
        navigate({
          to: '/shopping-lists/$id',
          params: { id: value.replace('shopping-list:', '') },
        })
        return
      }

      if (value.startsWith('store:')) {
        navigate({
          to: '/stores/$id',
          params: { id: value.replace('store:', '') },
        })
        return
      }

      if (value.startsWith('collection:')) {
        navigate({
          to: '/collections/$id',
          params: { id: value.replace('collection:', '') },
        })
        return
      }

      if (value === 'action:new-recipe') {
        navigate({ to: '/recipes/new' })
        return
      }

      if (value === 'action:import-url') {
        navigate({ to: '/recipes/import' })
        return
      }

      if (value === 'action:quick-add') {
        window.dispatchEvent(new CustomEvent('open-quick-add'))
        return
      }
    },
    [handleOpenChange, navigate],
  )

  const commandChildren = (
    <>
      <div className="relative">
        <CommandInput
          placeholder="Search or jump to..."
          value={query}
          onValueChange={setQuery}
        />
        {isFetching && (
          <div className="absolute right-4 top-1/2 -translate-y-1/2">
            <HugeiconsIcon
              icon={Loading03Icon}
              className="size-4 animate-spin text-muted-foreground"
            />
          </div>
        )}
      </div>
      <CommandList>
        {isSearching && !isFetching && !hasResults && (
          <CommandEmpty>No results found.</CommandEmpty>
        )}
        {isSearching && isFetching && !hasResults && (
          <CommandEmpty>Searching...</CommandEmpty>
        )}

        {/* Quick actions when not searching */}
        {!isSearching && (
          <>
            <CommandGroup heading="Navigation">
              <CommandItem
                value="nav:/recipes"
                onSelect={handleSelect}
              >
                <HugeiconsIcon icon={RestaurantIcon} />
                <span>Recipes</span>
                <CommandShortcut>G+R</CommandShortcut>
              </CommandItem>
              <CommandItem
                value="nav:/shopping-lists"
                onSelect={handleSelect}
              >
                <HugeiconsIcon icon={ShoppingCart01Icon} />
                <span>Shopping Lists</span>
                <CommandShortcut>G+L</CommandShortcut>
              </CommandItem>
            </CommandGroup>
            <CommandGroup heading="Actions">
              <CommandItem
                value="action:quick-add"
                onSelect={handleSelect}
              >
                <HugeiconsIcon icon={Add01Icon} />
                <span>Quick Add Items</span>
              </CommandItem>
              <CommandItem
                value="action:import-url"
                onSelect={handleSelect}
              >
                <HugeiconsIcon icon={FileImportIcon} />
                <span>Import from URL</span>
              </CommandItem>
            </CommandGroup>
          </>
        )}

        {/* Search results */}
        {isSearching && (recipes.data?.items.length ?? 0) > 0 && (
          <CommandGroup heading="Recipes">
            {recipes.data!.items.map((recipe) => (
              <CommandItem
                key={recipe.id}
                value={`recipe:${recipe.id}`}
                onSelect={handleSelect}
              >
                {recipe.imageUrl ? (
                  <img
                    src={recipe.imageUrl}
                    alt=""
                    className="size-8 shrink-0 rounded object-cover"
                  />
                ) : (
                  <div className="flex size-8 shrink-0 items-center justify-center">
                    <HugeiconsIcon icon={RestaurantIcon} />
                  </div>
                )}
                <div className="min-w-0 flex-1">
                  <span>{recipe.title}</span>
                  {recipe.description && (
                    <p className="line-clamp-1 text-xs text-muted-foreground">
                      {recipe.description}
                    </p>
                  )}
                </div>
              </CommandItem>
            ))}
          </CommandGroup>
        )}

        {isSearching && (shoppingLists.data?.items.length ?? 0) > 0 && (
          <CommandGroup heading="Shopping Lists">
            {shoppingLists.data!.items.map((list) => (
              <CommandItem
                key={list.id}
                value={`shopping-list:${list.id}`}
                onSelect={handleSelect}
              >
                <HugeiconsIcon
                  icon={ShoppingCart01Icon}
                  className="shrink-0"
                />
                <span>{list.name}</span>
                <Badge variant="outline" className="ml-auto text-xs">
                  {list.status}
                </Badge>
              </CommandItem>
            ))}
          </CommandGroup>
        )}

        {isSearching && (stores.data?.items.length ?? 0) > 0 && (
          <CommandGroup heading="Stores">
            {stores.data!.items.map((store) => (
              <CommandItem
                key={store.id}
                value={`store:${store.id}`}
                onSelect={handleSelect}
              >
                <HugeiconsIcon
                  icon={Store01Icon}
                  className="shrink-0"
                />
                <div className="min-w-0 flex-1">
                  <span>{store.name}</span>
                  {store.address && (
                    <p className="line-clamp-1 text-xs text-muted-foreground">
                      {store.address}
                    </p>
                  )}
                </div>
              </CommandItem>
            ))}
          </CommandGroup>
        )}

        {isSearching && (collections.data?.items.length ?? 0) > 0 && (
          <CommandGroup heading="Collections">
            {collections.data!.items.map((collection) => (
              <CommandItem
                key={collection.id}
                value={`collection:${collection.id}`}
                onSelect={handleSelect}
              >
                <HugeiconsIcon
                  icon={Layers01Icon}
                  className="shrink-0"
                />
                <span>{collection.name}</span>
                <span className="ml-auto text-xs text-muted-foreground">
                  {collection.items.length}{' '}
                  {collection.items.length === 1 ? 'item' : 'items'}
                </span>
              </CommandItem>
            ))}
          </CommandGroup>
        )}
      </CommandList>
    </>
  )

  if (isMobile) {
    return (
      <Drawer open={open} onOpenChange={handleOpenChange}>
        <DrawerContent>
          <DrawerTitle className="sr-only">Command Palette</DrawerTitle>
          <DrawerDescription className="sr-only">
            Search for a command to run...
          </DrawerDescription>
          <Command shouldFilter={!isSearching}>
            {commandChildren}
          </Command>
        </DrawerContent>
      </Drawer>
    )
  }

  return (
    <CommandDialog open={open} onOpenChange={handleOpenChange}>
      <Command shouldFilter={!isSearching}>
        {commandChildren}
      </Command>
    </CommandDialog>
  )
}
