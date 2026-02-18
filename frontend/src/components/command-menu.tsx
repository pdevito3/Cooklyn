import { useState, useCallback, useRef, useEffect, useMemo } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useInfiniteQuery } from '@tanstack/react-query'
import {
  RestaurantIcon,
  ShoppingCart01Icon,
  Store01Icon,
  Layers01Icon,
  FileImportIcon,
  Add01Icon,
  Loading03Icon,
  Search01Icon,
  Cancel01Icon,
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
import { useRecentSearches } from '@/domain/recent-searches/apis/get-recent-searches'
import {
  useAddRecentSearch,
  useDeleteRecentSearch,
  useClearRecentSearches,
} from '@/domain/recent-searches/apis/recent-search-mutations'
import type { RecentSearchDto } from '@/domain/recent-searches/types'

const PAGE_SIZE = 15

const resourceTypeIcons: Record<string, typeof RestaurantIcon> = {
  recipe: RestaurantIcon,
  'shopping-list': ShoppingCart01Icon,
  store: Store01Icon,
  collection: Layers01Icon,
}

interface CommandMenuProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function CommandMenu({ open, onOpenChange }: CommandMenuProps) {
  const navigate = useNavigate()
  const isMobile = useIsMobile()
  const [query, setQuery] = useState('')
  const debouncedQuery = useDebouncedValue(query, 250)
  const selectionMadeRef = useRef(false)
  const lastSearchQueryRef = useRef('')
  const sentinelRef = useRef<HTMLDivElement>(null)
  const listRef = useRef<HTMLDivElement>(null)

  const isSearching = debouncedQuery.length >= 2
  const searchFilter = (field: string) =>
    `${field} @=* "${debouncedQuery}"`

  const recentSearches = useRecentSearches()
  const addRecentSearch = useAddRecentSearch()
  const deleteRecentSearch = useDeleteRecentSearch()
  const clearRecentSearches = useClearRecentSearches()

  const recipes = useInfiniteQuery({
    queryKey: ['command-search', debouncedQuery, 'recipes'],
    queryFn: ({ pageParam }) =>
      getRecipes({ filters: searchFilter('title'), pageSize: PAGE_SIZE, pageNumber: pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.pagination.hasNext ? lastPage.pagination.pageNumber + 1 : undefined,
    enabled: isSearching,
    staleTime: 30_000,
  })

  const shoppingLists = useInfiniteQuery({
    queryKey: ['command-search', debouncedQuery, 'shopping-lists'],
    queryFn: ({ pageParam }) =>
      getShoppingLists({ filters: searchFilter('name'), pageSize: PAGE_SIZE, pageNumber: pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.pagination.hasNext ? lastPage.pagination.pageNumber + 1 : undefined,
    enabled: isSearching,
    staleTime: 30_000,
  })

  const stores = useInfiniteQuery({
    queryKey: ['command-search', debouncedQuery, 'stores'],
    queryFn: ({ pageParam }) =>
      getStores({ filters: searchFilter('name'), pageSize: PAGE_SIZE, pageNumber: pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.pagination.hasNext ? lastPage.pagination.pageNumber + 1 : undefined,
    enabled: isSearching,
    staleTime: 30_000,
  })

  const collections = useInfiniteQuery({
    queryKey: ['command-search', debouncedQuery, 'collections'],
    queryFn: ({ pageParam }) =>
      getItemCollections({ filters: searchFilter('name'), pageSize: PAGE_SIZE, pageNumber: pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) =>
      lastPage.pagination.hasNext ? lastPage.pagination.pageNumber + 1 : undefined,
    enabled: isSearching,
    staleTime: 30_000,
  })

  const allRecipes = useMemo(
    () => recipes.data?.pages.flatMap((p) => p.items) ?? [],
    [recipes.data],
  )
  const allShoppingLists = useMemo(
    () => shoppingLists.data?.pages.flatMap((p) => p.items) ?? [],
    [shoppingLists.data],
  )
  const allStores = useMemo(
    () => stores.data?.pages.flatMap((p) => p.items) ?? [],
    [stores.data],
  )
  const allCollections = useMemo(
    () => collections.data?.pages.flatMap((p) => p.items) ?? [],
    [collections.data],
  )

  const isFetching =
    recipes.isFetching ||
    shoppingLists.isFetching ||
    stores.isFetching ||
    collections.isFetching

  const isFetchingNextPage =
    recipes.isFetchingNextPage ||
    shoppingLists.isFetchingNextPage ||
    stores.isFetchingNextPage ||
    collections.isFetchingNextPage

  const hasResults =
    allRecipes.length > 0 ||
    allShoppingLists.length > 0 ||
    allStores.length > 0 ||
    allCollections.length > 0

  // Track the last meaningful search query (>= 2 chars) so we can save it
  // even if the user clears the input before closing the palette
  useEffect(() => {
    if (query.length >= 2) {
      lastSearchQueryRef.current = query
    }
  }, [query])

  // Save the query as a recent search when the user clears the input
  // (transitions from searching → not searching while palette is still open)
  const wasSearchingRef = useRef(false)
  useEffect(() => {
    if (isSearching) {
      wasSearchingRef.current = true
    } else if (wasSearchingRef.current) {
      wasSearchingRef.current = false
      const lastQuery = lastSearchQueryRef.current
      if (lastQuery.length >= 2) {
        addRecentSearch.mutate({
          searchType: 'query',
          searchText: lastQuery,
        })
        lastSearchQueryRef.current = ''
      }
    }
  }, [isSearching, addRecentSearch])

  const {
    hasNextPage: recipesHasNext,
    fetchNextPage: fetchNextRecipes,
  } = recipes
  const {
    hasNextPage: shoppingListsHasNext,
    fetchNextPage: fetchNextShoppingLists,
  } = shoppingLists
  const {
    hasNextPage: storesHasNext,
    fetchNextPage: fetchNextStores,
  } = stores
  const {
    hasNextPage: collectionsHasNext,
    fetchNextPage: fetchNextCollections,
  } = collections

  // Infinite scroll: observe sentinel within the CommandList scroll container
  useEffect(() => {
    const sentinel = sentinelRef.current
    const scrollRoot = listRef.current
    if (!sentinel || !scrollRoot || !isSearching) return

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (!entry.isIntersecting || isFetchingNextPage) return
        if (recipesHasNext) fetchNextRecipes()
        if (shoppingListsHasNext) fetchNextShoppingLists()
        if (storesHasNext) fetchNextStores()
        if (collectionsHasNext) fetchNextCollections()
      },
      { root: scrollRoot, rootMargin: '200px' },
    )
    observer.observe(sentinel)
    return () => observer.disconnect()
  }, [
    isSearching,
    isFetchingNextPage,
    recipesHasNext,
    shoppingListsHasNext,
    storesHasNext,
    collectionsHasNext,
    fetchNextRecipes,
    fetchNextShoppingLists,
    fetchNextStores,
    fetchNextCollections,
  ])

  const handleOpenChange = useCallback(
    (value: boolean) => {
      if (!value) {
        // Save the last meaningful query if it wasn't already saved by the
        // searching→not-searching transition (e.g. user closes while still typing)
        const lastQuery = lastSearchQueryRef.current
        if (lastQuery.length >= 2) {
          addRecentSearch.mutate({
            searchType: 'query',
            searchText: lastQuery,
          })
        }
        setQuery('')
        selectionMadeRef.current = false
        lastSearchQueryRef.current = ''
        wasSearchingRef.current = false
      }
      onOpenChange(value)
    },
    [onOpenChange, addRecentSearch],
  )

  const getSearchTextForValue = useCallback(
    (value: string): { searchText: string; resourceType: string; resourceId: string } | null => {
      if (value.startsWith('recipe:')) {
        const id = value.replace('recipe:', '')
        const recipe = allRecipes.find((r) => r.id === id)
        if (recipe) return { searchText: recipe.title, resourceType: 'recipe', resourceId: id }
      }
      if (value.startsWith('shopping-list:')) {
        const id = value.replace('shopping-list:', '')
        const list = allShoppingLists.find((l) => l.id === id)
        if (list) return { searchText: list.name, resourceType: 'shopping-list', resourceId: id }
      }
      if (value.startsWith('store:')) {
        const id = value.replace('store:', '')
        const store = allStores.find((s) => s.id === id)
        if (store) return { searchText: store.name, resourceType: 'store', resourceId: id }
      }
      if (value.startsWith('collection:')) {
        const id = value.replace('collection:', '')
        const col = allCollections.find((c) => c.id === id)
        if (col) return { searchText: col.name, resourceType: 'collection', resourceId: id }
      }
      return null
    },
    [allRecipes, allShoppingLists, allStores, allCollections],
  )

  const navigateToResource = useCallback(
    (resourceType: string, resourceId: string) => {
      if (resourceType === 'recipe') {
        navigate({ to: '/recipes/$id', params: { id: resourceId } })
      } else if (resourceType === 'shopping-list') {
        navigate({ to: '/shopping-lists/$id', params: { id: resourceId } })
      } else if (resourceType === 'store') {
        navigate({ to: '/stores/$id', params: { id: resourceId } })
      } else if (resourceType === 'collection') {
        navigate({ to: '/collections/$id', params: { id: resourceId } })
      }
    },
    [navigate],
  )

  const handleSelect = useCallback(
    (value: string) => {
      selectionMadeRef.current = true

      // Track selection from search results
      const searchInfo = getSearchTextForValue(value)
      if (searchInfo) {
        addRecentSearch.mutate({
          searchType: 'selection',
          searchText: searchInfo.searchText,
          resourceType: searchInfo.resourceType,
          resourceId: searchInfo.resourceId,
        })
      }

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
    [handleOpenChange, navigate, getSearchTextForValue, addRecentSearch],
  )

  const handleRecentSelect = useCallback(
    (entry: RecentSearchDto) => {
      selectionMadeRef.current = true
      if (entry.searchType === 'query') {
        // Populate the search input to re-run the search
        setQuery(entry.searchText)
      } else if (entry.resourceType && entry.resourceId) {
        // Navigate to the resource
        handleOpenChange(false)
        navigateToResource(entry.resourceType, entry.resourceId)
      }
    },
    [handleOpenChange, navigateToResource],
  )

  const handleDeleteRecent = useCallback(
    (e: React.MouseEvent, id: string) => {
      e.stopPropagation()
      e.preventDefault()
      deleteRecentSearch.mutate(id)
    },
    [deleteRecentSearch],
  )

  const recentEntries = useMemo(
    () => recentSearches.data ?? [],
    [recentSearches.data],
  )

  // Filter recent searches that match the current query for inline display
  const matchingRecentEntries = useMemo(() => {
    if (!isSearching || recentEntries.length === 0) return []
    const lowerQuery = debouncedQuery.toLowerCase()
    return recentEntries.filter((entry) =>
      entry.searchText.toLowerCase().includes(lowerQuery),
    )
  }, [isSearching, debouncedQuery, recentEntries])

  const commandChildren = (
    <>
      <div className="relative">
        <CommandInput
          placeholder="Search or jump to..."
          value={query}
          onValueChange={setQuery}
        />
        {isFetching && !isFetchingNextPage && (
          <div className="absolute right-4 top-1/2 -translate-y-1/2">
            <HugeiconsIcon
              icon={Loading03Icon}
              className="size-4 animate-spin text-muted-foreground"
            />
          </div>
        )}
      </div>
      <CommandList ref={listRef}>
        {isSearching && !isFetching && !hasResults && matchingRecentEntries.length === 0 && (
          <CommandEmpty>No results found.</CommandEmpty>
        )}
        {isSearching && isFetching && !hasResults && matchingRecentEntries.length === 0 && (
          <CommandEmpty>Searching...</CommandEmpty>
        )}

        {/* Recent searches when not searching (show top 5) */}
        {!isSearching && recentEntries.length > 0 && (
          <CommandGroup
            heading={
              <div className="flex items-center justify-between">
                <span>Recent</span>
                <button
                  type="button"
                  className="text-xs font-normal text-muted-foreground hover:text-foreground"
                  onClick={(e) => {
                    e.stopPropagation()
                    clearRecentSearches.mutate()
                  }}
                >
                  Clear all
                </button>
              </div>
            }
          >
            {recentEntries.slice(0, 5).map((entry) => (
              <CommandItem
                key={entry.id}
                value={`recent:${entry.id}`}
                onSelect={() => handleRecentSelect(entry)}
              >
                <HugeiconsIcon
                  icon={
                    entry.searchType === 'query'
                      ? Search01Icon
                      : (entry.resourceType && resourceTypeIcons[entry.resourceType]) || Search01Icon
                  }
                  className="shrink-0"
                />
                <span className="flex-1 truncate">{entry.searchText}</span>
                <button
                  type="button"
                  className="ml-auto shrink-0 rounded p-0.5 text-muted-foreground opacity-0 hover:text-foreground group-data-[active]/command-item:opacity-100"
                  onClick={(e) => handleDeleteRecent(e, entry.id)}
                  tabIndex={-1}
                >
                  <HugeiconsIcon icon={Cancel01Icon} className="size-3.5" />
                </button>
              </CommandItem>
            ))}
          </CommandGroup>
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

        {/* Matching recent searches while searching */}
        {isSearching && matchingRecentEntries.length > 0 && (
          <CommandGroup heading="Recent">
            {matchingRecentEntries.map((entry) => (
              <CommandItem
                key={entry.id}
                value={`recent:${entry.id}`}
                onSelect={() => handleRecentSelect(entry)}
              >
                <HugeiconsIcon
                  icon={
                    entry.searchType === 'query'
                      ? Search01Icon
                      : (entry.resourceType && resourceTypeIcons[entry.resourceType]) || Search01Icon
                  }
                  className="shrink-0"
                />
                <span className="flex-1 truncate">{entry.searchText}</span>
              </CommandItem>
            ))}
          </CommandGroup>
        )}

        {/* Search results */}
        {isSearching && allRecipes.length > 0 && (
          <CommandGroup heading="Recipes">
            {allRecipes.map((recipe) => (
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

        {isSearching && allShoppingLists.length > 0 && (
          <CommandGroup heading="Shopping Lists">
            {allShoppingLists.map((list) => (
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

        {isSearching && allStores.length > 0 && (
          <CommandGroup heading="Stores">
            {allStores.map((store) => (
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

        {isSearching && allCollections.length > 0 && (
          <CommandGroup heading="Collections">
            {allCollections.map((collection) => (
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

        {/* Infinite scroll sentinel + loading indicator */}
        {isSearching && (
          <>
            <div ref={sentinelRef} className="h-1" />
            {isFetchingNextPage && (
              <div className="flex items-center justify-center py-3">
                <HugeiconsIcon
                  icon={Loading03Icon}
                  className="size-4 animate-spin text-muted-foreground"
                />
                <span className="ml-2 text-xs text-muted-foreground">
                  Loading more...
                </span>
              </div>
            )}
          </>
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
