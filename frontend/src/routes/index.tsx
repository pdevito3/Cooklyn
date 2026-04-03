import { useRef, useState } from 'react'
import { createFileRoute, Link, useNavigate } from '@tanstack/react-router'
import { useHotkeys } from 'react-hotkeys-hook'
import { format, startOfWeek, endOfWeek, addDays, isToday } from 'date-fns'

import { useMealPlanCalendar } from '@/domain/meal-plans/apis/get-meal-plan-calendar'
import { useShoppingLists } from '@/domain/shopping-lists/apis/get-shopping-lists'
import { useAddShoppingListItem } from '@/domain/shopping-lists/apis/shopping-list-mutations'
import { useRecipes } from '@/domain/recipes/apis/get-recipes'
import { useStores } from '@/domain/stores/apis/get-stores'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import { Kbd } from '@/components/ui/kbd'
import { cn } from '@/lib/utils'

export const Route = createFileRoute('/')({
  component: Dashboard,
})

function getGreeting(): string {
  const hour = new Date().getHours()
  if (hour < 12) return 'Good morning'
  if (hour < 18) return 'Good afternoon'
  return 'Good evening'
}

function Dashboard() {
  const navigate = useNavigate()
  const searchRef = useRef<HTMLInputElement>(null)
  const [searchQuery, setSearchQuery] = useState('')

  // Week range (Monday–Sunday)
  const now = new Date()
  const weekStart = startOfWeek(now, { weekStartsOn: 1 })
  const weekEnd = endOfWeek(now, { weekStartsOn: 1 })
  const startDate = format(weekStart, 'yyyy-MM-dd')
  const endDate = format(weekEnd, 'yyyy-MM-dd')

  const { data: days = [], isLoading: mealPlanLoading } = useMealPlanCalendar(
    startDate,
    endDate,
  )
  const { data: listsData, isLoading: listsLoading } = useShoppingLists({
    pageSize: 100,
  })
  const { data: recipesData } = useRecipes({ pageSize: 1 })
  const { data: storesData } = useStores({ pageSize: 100 })

  const activeLists =
    listsData?.items.filter((l) => l.status !== 'Completed') ?? []
  const storeMap = new Map(storesData?.items.map((s) => [s.id, s.name]) ?? [])

  const totalRecipes = recipesData?.pagination.totalCount ?? 0
  const mealsThisWeek = days.reduce((sum, d) => sum + d.entries.length, 0)

  // Build full 7-day grid so empty days show too
  const weekDays = Array.from({ length: 7 }, (_, i) => {
    const date = addDays(weekStart, i)
    const key = format(date, 'yyyy-MM-dd')
    const dayData = days.find((d) => d.date === key)
    return { date, key, entries: dayData?.entries ?? [] }
  })

  // Hotkeys
  useHotkeys(
    '/',
    (e) => {
      e.preventDefault()
      searchRef.current?.focus()
    },
    { preventDefault: true },
  )
  useHotkeys('p', () => navigate({ to: '/meal-plan' }))
  useHotkeys('r', () => navigate({ to: '/recipes' }))

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    navigate({ to: '/recipes' })
  }

  return (
    <div className="min-h-screen bg-background">
      <main className="mx-auto max-w-6xl px-4 py-8">
        {/* Header */}
        <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <h1 className="text-3xl font-bold tracking-tight">
            {getGreeting()}
          </h1>
          <form onSubmit={handleSearch} className="relative w-full sm:w-72">
            <Input
              ref={searchRef}
              placeholder="Search recipes…"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pr-10"
            />
            <span className="pointer-events-none absolute right-2.5 top-1/2 -translate-y-1/2">
              <Kbd>/</Kbd>
            </span>
          </form>
        </div>

        {/* Main grid */}
        <div className="grid gap-6 lg:grid-cols-2">
          {/* This Week's Meal Plan */}
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>This Week's Meals</CardTitle>
              <Button variant="ghost" size="sm" render={<Link to="/meal-plan" />}>
                Plan meals
              </Button>
            </CardHeader>
            <CardContent>
              {mealPlanLoading ? (
                <div className="space-y-3">
                  {Array.from({ length: 7 }, (_, i) => (
                    <Skeleton key={i} className="h-8 w-full" />
                  ))}
                </div>
              ) : (
                <div className="space-y-1">
                  {weekDays.map(({ date, key, entries }) => (
                    <div
                      key={key}
                      className={cn(
                        'flex gap-3 rounded-md px-2 py-1.5',
                        isToday(date) && 'bg-accent',
                      )}
                    >
                      <span
                        className={cn(
                          'w-16 shrink-0 text-sm font-medium',
                          isToday(date)
                            ? 'text-foreground'
                            : 'text-muted-foreground',
                        )}
                      >
                        {format(date, 'EEE d')}
                      </span>
                      <div className="min-w-0 flex-1">
                        {entries.length > 0 ? (
                          <div className="space-y-0.5">
                            {entries.map((entry) => (
                              <div
                                key={entry.id}
                                className="flex items-center gap-2"
                              >
                                {entry.imageUrl && (
                                  <img
                                    src={entry.imageUrl}
                                    alt=""
                                    className="h-5 w-5 rounded object-cover"
                                  />
                                )}
                                <span className="truncate text-sm">
                                  {entry.title}
                                </span>
                              </div>
                            ))}
                          </div>
                        ) : (
                          <span className="text-sm text-muted-foreground/50">
                            No meals planned
                          </span>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>

          {/* Active Shopping Lists */}
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>Active Shopping Lists</CardTitle>
              <Button
                variant="ghost"
                size="sm"
                render={<Link to="/shopping-lists" />}
              >
                View all
              </Button>
            </CardHeader>
            <CardContent>
              {listsLoading ? (
                <div className="space-y-4">
                  {Array.from({ length: 3 }, (_, i) => (
                    <Skeleton key={i} className="h-16 w-full" />
                  ))}
                </div>
              ) : activeLists.length > 0 ? (
                <div className="space-y-4">
                  {activeLists.map((list) => (
                    <ShoppingListCard
                      key={list.id}
                      list={list}
                      storeName={
                        list.storeId ? storeMap.get(list.storeId) : undefined
                      }
                    />
                  ))}
                </div>
              ) : (
                <div className="py-6 text-center text-sm text-muted-foreground">
                  <p>No active shopping lists</p>
                  <Button
                    variant="link"
                    size="sm"
                    className="mt-1"
                    render={<Link to="/shopping-lists" />}
                  >
                    Create one
                  </Button>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Bottom row */}
        <div className="mt-6 grid gap-6 sm:grid-cols-2">
          {/* Quick Actions */}
          <Card>
            <CardHeader>
              <CardTitle>Quick Actions</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex flex-wrap gap-2">
                <Button variant="outline" render={<Link to="/meal-plan" />}>
                  Meal Plan
                  <Kbd>P</Kbd>
                </Button>
                <Button variant="outline" render={<Link to="/recipes" />}>
                  Recipes
                  <Kbd>R</Kbd>
                </Button>
              </div>
            </CardContent>
          </Card>

          {/* Stats */}
          <Card>
            <CardHeader>
              <CardTitle>At a Glance</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex flex-wrap gap-x-6 gap-y-2 text-sm">
                <span>
                  <span className="font-semibold">{totalRecipes}</span>{' '}
                  <span className="text-muted-foreground">recipes</span>
                </span>
                <span>
                  <span className="font-semibold">{mealsThisWeek}</span>{' '}
                  <span className="text-muted-foreground">
                    meals this week
                  </span>
                </span>
                <span>
                  <span className="font-semibold">{activeLists.length}</span>{' '}
                  <span className="text-muted-foreground">active lists</span>
                </span>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  )
}

function ShoppingListCard({
  list,
  storeName,
}: {
  list: {
    id: string
    name: string
    itemCount: number
    checkedCount: number
  }
  storeName?: string
}) {
  const addItem = useAddShoppingListItem()
  const [newItem, setNewItem] = useState('')

  const progress =
    list.itemCount > 0
      ? Math.round((list.checkedCount / list.itemCount) * 100)
      : 0

  const handleAdd = (e: React.FormEvent) => {
    e.preventDefault()
    const name = newItem.trim()
    if (!name) return
    addItem.mutate(
      {
        shoppingListId: list.id,
        dto: {
          name,
          quantity: null,
          unit: null,
          storeSectionId: null,
          notes: null,
        },
      },
      { onSuccess: () => setNewItem('') },
    )
  }

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between gap-2">
        <Link
          to="/shopping-lists/$id"
          params={{ id: list.id }}
          className="truncate font-medium hover:underline"
        >
          {list.name}
        </Link>
        <span className="shrink-0 text-xs text-muted-foreground">
          {list.checkedCount}/{list.itemCount}
        </span>
      </div>
      {storeName && (
        <p className="text-xs text-muted-foreground">{storeName}</p>
      )}
      {/* Progress bar */}
      <div className="h-1.5 w-full rounded-full bg-muted">
        <div
          className="h-full rounded-full bg-primary transition-all duration-300"
          style={{ width: `${progress}%` }}
        />
      </div>
      {/* Inline add */}
      <form onSubmit={handleAdd} className="flex gap-1.5">
        <Input
          placeholder="Add item…"
          value={newItem}
          onChange={(e) => setNewItem(e.target.value)}
          className="h-7 text-xs"
        />
        <Button
          type="submit"
          variant="ghost"
          size="xs"
          disabled={addItem.isPending || !newItem.trim()}
        >
          Add
        </Button>
      </form>
    </div>
  )
}
