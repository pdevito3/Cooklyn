import { useMemo, useState } from 'react'
import { Delete02Icon, FolderAddIcon, FloppyDiskIcon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import {
  Combobox,
  ComboboxInput,
  ComboboxItem,
  ComboboxItemIndicator,
  ComboboxClear,
  ComboboxContent,
} from '@/components/ui/combobox'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'
import { cn } from '@/lib/utils'
import { FilterEditModal } from './controls/filter-edit-modal'
import { FilterBadge } from './filter-badge'
import { FilterGroup } from './filter-group'
import { FilterPropertyMenu } from './filter-property-menu'
import type {
  Filter,
  FilterBuilderProps,
  FilterState,
  SavedFilterItem,
} from './types'
import { isFilter } from './types'
import { canCreateGroup } from './utils/depth'
import { toQueryKitString } from './utils/querykit-converter'
import { LogicalOperators } from './utils/operators'
import { useFilterBuilderReducer } from './use-filter-builder-reducer'

type ActiveSource = {
  type: 'preset' | 'saved'
  label?: string
  id?: string
  filterIds: Set<string>
}

function getSourceStatus(
  source: ActiveSource | null,
  currentState: FilterState,
): 'active' | 'partial' | 'inactive' {
  if (!source) return 'inactive'
  const currentIds = new Set(currentState.filters.map((f) => f.id))
  const allPresent = [...source.filterIds].every((id) => currentIds.has(id))
  if (!allPresent) return 'inactive'
  if (currentIds.size === source.filterIds.size) return 'active'
  return 'partial'
}

export function FilterBuilder({
  filterOptions,
  presets = [],
  savedFilters = [],
  onSaveFilter,
  onDeleteSavedFilter,
  onUpdateSavedFilter,
  onChange,
  initialState,
  className,
}: FilterBuilderProps) {
  const { state, actions } = useFilterBuilderReducer({ initialState, onChange })

  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set())
  const [editingFilter, setEditingFilter] = useState<Filter | null>(null)
  const [editModalOpen, setEditModalOpen] = useState(false)
  const [isGroupingMode, setIsGroupingMode] = useState(false)
  const [saveFilterName, setSaveFilterName] = useState('')
  const [savePopoverOpen, setSavePopoverOpen] = useState(false)
  const [selectedSavedFilterId, setSelectedSavedFilterId] = useState<
    string | null
  >(null)
  const [activeSource, setActiveSource] = useState<ActiveSource | null>(null)

  const selectedSavedFilter = useMemo(
    () =>
      (savedFilters ?? []).find((sf) => sf.id === selectedSavedFilterId) ??
      null,
    [savedFilters, selectedSavedFilterId],
  )

  const sourceStatus = useMemo(
    () => getSourceStatus(activeSource, state),
    [activeSource, state],
  )

  const isSavedActive =
    activeSource?.type === 'saved' &&
    selectedSavedFilter != null &&
    activeSource.id === selectedSavedFilter.id
  const savedStatus = isSavedActive ? sourceStatus : 'inactive'

  const hasDiverged = useMemo(() => {
    if (!selectedSavedFilter) return false
    return (
      JSON.stringify(state) !==
      JSON.stringify(selectedSavedFilter.filterState)
    )
  }, [selectedSavedFilter, state])

  const handleAddFilter = (filter: Omit<Filter, 'id'>) => {
    const newFilter: Filter = {
      ...filter,
      id: crypto.randomUUID(),
    }
    actions.addFilter(newFilter)
  }

  const handleRemoveFilter = (filterId: string) => {
    actions.removeFilter(filterId)
    setSelectedIds((prev) => {
      const next = new Set(prev)
      next.delete(filterId)
      return next
    })
  }

  const handleToggleLogicalOperator = () => {
    actions.toggleRootOperator()
  }

  const handleToggleGroupOperator = (groupId: string) => {
    actions.toggleGroupOperator(groupId)
  }

  const handleUngroup = (groupId: string) => {
    actions.ungroup(groupId)
    setSelectedIds((prev) => {
      const next = new Set(prev)
      next.delete(groupId)
      return next
    })
  }

  const handleEditFilter = (filter: Filter) => {
    setEditingFilter(filter)
    setEditModalOpen(true)
  }

  const handleUpdateFilter = (updatedFilter: Omit<Filter, 'id'>) => {
    if (!editingFilter) return

    actions.updateFilter(editingFilter.id, updatedFilter)
    setEditModalOpen(false)
    setEditingFilter(null)
  }

  const handleToggleSelection = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev)
      if (next.has(id)) {
        next.delete(id)
      } else {
        next.add(id)
      }
      return next
    })
  }

  const handleEnterGroupingMode = () => {
    setIsGroupingMode(true)
  }

  const handleCancelGrouping = () => {
    setIsGroupingMode(false)
    setSelectedIds(new Set())
  }

  const handleClearAll = () => {
    actions.clearAll()
    setSelectedIds(new Set())
    setActiveSource(null)
    setSelectedSavedFilterId(null)
  }

  const handleCreateGroup = () => {
    const ids = Array.from(selectedIds)
    const { canCreate, reason } = canCreateGroup(state, ids)

    if (!canCreate) {
      alert(reason || 'Cannot create group')
      return
    }

    actions.createGroup(ids, LogicalOperators.AND)
    setSelectedIds(new Set())
    setIsGroupingMode(false)
  }

  const hasFilters = state.filters.length > 0
  const hasMultipleFilters = state.filters.length > 1
  const selectedCount = selectedIds.size
  const canGroup = selectedCount >= 2

  return (
    <div className={cn('space-y-2', className)}>
      <div className="flex items-center gap-2 flex-wrap">
        <FilterPropertyMenu
          options={filterOptions}
          onAddFilter={handleAddFilter}
        />

        {/* Grouping Controls */}
        {hasFilters && !isGroupingMode && (
          <>
            <Button
              variant="outline"
              size="sm"
              onClick={handleEnterGroupingMode}
            >
              <HugeiconsIcon icon={FolderAddIcon} className="size-4 mr-2" />
              Group Filters
            </Button>

            <Button variant="outline" size="sm" onClick={handleClearAll}>
              Clear
            </Button>

            {onSaveFilter && (
              <Popover open={savePopoverOpen} onOpenChange={setSavePopoverOpen}>
                <PopoverTrigger
                  render={
                    <Button variant="outline" size="sm">
                      <HugeiconsIcon
                        icon={FloppyDiskIcon}
                        className="size-4 mr-2"
                      />
                      Save
                    </Button>
                  }
                />
                <PopoverContent align="start" className="w-64">
                  <form
                    onSubmit={(e) => {
                      e.preventDefault()
                      const name = saveFilterName.trim()
                      if (name) {
                        onSaveFilter(name)
                        setSaveFilterName('')
                        setSavePopoverOpen(false)
                      }
                    }}
                    className="flex flex-col gap-2"
                  >
                    <label className="text-sm font-medium">Filter name</label>
                    <Input
                      value={saveFilterName}
                      onChange={(e) => setSaveFilterName(e.target.value)}
                      placeholder="e.g. My Top Rated"
                      autoFocus
                    />
                    <Button
                      type="submit"
                      size="sm"
                      disabled={!saveFilterName.trim()}
                    >
                      Save filter
                    </Button>
                  </form>
                </PopoverContent>
              </Popover>
            )}
          </>
        )}

        {isGroupingMode && (
          <>
            <Button
              variant="default"
              size="sm"
              onClick={handleCreateGroup}
              disabled={!canGroup}
            >
              <HugeiconsIcon icon={FolderAddIcon} className="size-4 mr-2" />
              {selectedCount > 0
                ? `Group Selected (${selectedCount})`
                : 'Group Filters'}
            </Button>

            <Button variant="outline" size="sm" onClick={handleCancelGrouping}>
              Cancel
            </Button>
          </>
        )}

        {/* Logical operator toggle button */}
        {hasMultipleFilters && (
          <Button
            variant="ghost"
            size="sm"
            onClick={handleToggleLogicalOperator}
            className="h-7 px-2 text-xs"
          >
            Switch to{' '}
            {state.rootLogicalOperator === LogicalOperators.AND
              ? LogicalOperators.OR
              : LogicalOperators.AND}
          </Button>
        )}
      </div>

      <div className="flex flex-wrap items-start gap-3">
        {state.filters.map((item, index) => (
          <div key={item.id} className="flex items-center gap-1.5">
            {isFilter(item) ? (
              <div className="flex items-center gap-1.5">
                {isGroupingMode && (
                  <Checkbox
                    isSelected={selectedIds.has(item.id)}
                    onChange={() => handleToggleSelection(item.id)}
                    className="shrink-0"
                  />
                )}
                <FilterBadge
                  filter={item}
                  onRemove={() => handleRemoveFilter(item.id)}
                  onEdit={() => handleEditFilter(item)}
                />
              </div>
            ) : (
              <div className="w-full">
                <FilterGroup
                  group={item}
                  depth={1}
                  selectedIds={selectedIds}
                  onToggleSelection={handleToggleSelection}
                  onToggleOperator={handleToggleGroupOperator}
                  onRemove={handleRemoveFilter}
                  onUngroup={handleUngroup}
                  onRemoveFilter={handleRemoveFilter}
                  onEditFilter={handleEditFilter}
                  filterOptions={filterOptions}
                  showCheckbox={isGroupingMode}
                />
              </div>
            )}

            {/* Show logical operator between items */}
            {index < state.filters.length - 1 && (
              <span className="text-sm text-muted-foreground font-medium px-1">
                {state.rootLogicalOperator}
              </span>
            )}
          </div>
        ))}
      </div>

      {/* Quick filter presets */}
      {presets.length > 0 && (
        <div className="flex gap-2 items-center flex-wrap">
          <span className="text-sm text-muted-foreground">Quick filters:</span>
          {presets.map((preset) => {
            const isThisActive =
              activeSource?.type === 'preset' &&
              activeSource.label === preset.label
            const status = isThisActive ? sourceStatus : 'inactive'

            return (
              <Button
                key={preset.label}
                variant={status !== 'inactive' ? 'default' : 'outline'}
                size="sm"
                className={status === 'partial' ? 'opacity-60' : ''}
                onClick={() => {
                  actions.applyPreset(preset.filter)
                  setSelectedIds(new Set())
                  setSelectedSavedFilterId(null)
                  setActiveSource({
                    type: 'preset',
                    label: preset.label,
                    filterIds: new Set(
                      preset.filter.filters.map((f) => f.id),
                    ),
                  })
                }}
              >
                {preset.label}
              </Button>
            )
          })}
        </div>
      )}

      {/* Saved filters */}
      {savedFilters.length > 0 && (
        <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
          <span className="text-sm text-muted-foreground shrink-0">
            Saved filters:
          </span>
          <div className="flex items-center gap-1.5 w-full sm:w-auto">
            <Combobox
              items={savedFilters}
              value={selectedSavedFilter}
              onValueChange={(newValue) => {
                if (newValue) {
                  setSelectedSavedFilterId(newValue.id)
                  actions.applyPreset(newValue.filterState)
                  setSelectedIds(new Set())
                  setActiveSource({
                    type: 'saved',
                    id: newValue.id,
                    filterIds: new Set(
                      newValue.filterState.filters.map((f) => f.id),
                    ),
                  })
                } else {
                  setSelectedSavedFilterId(null)
                  setActiveSource(null)
                  actions.clearAll()
                  setSelectedIds(new Set())
                }
              }}
              itemToStringValue={(item) => item?.name ?? ''}
              itemToStringLabel={(item) => item?.name ?? ''}
              isItemEqualToValue={(a, b) => a?.id === b?.id}
            >
              <div className="relative">
                <ComboboxInput
                  placeholder="Search saved filters..."
                  autoComplete="off"
                  className={cn(
                    'w-full sm:w-56',
                    selectedSavedFilter && 'pr-7',
                    savedStatus === 'active' &&
                      'border-primary ring-1 ring-primary/50',
                    savedStatus === 'partial' &&
                      'border-primary/50 ring-1 ring-primary/30',
                  )}
                />
                {selectedSavedFilter && (
                  <ComboboxClear
                    className="absolute right-2 top-1/2 -translate-y-1/2"
                    onClick={() => {
                      setSelectedSavedFilterId(null)
                      setActiveSource(null)
                      actions.clearAll()
                      setSelectedIds(new Set())
                    }}
                  />
                )}
              </div>
              <ComboboxContent emptyMessage="No saved filters found.">
                {(item: SavedFilterItem) => (
                  <ComboboxItem key={item.id} value={item}>
                    {item.name}
                    <ComboboxItemIndicator />
                  </ComboboxItem>
                )}
              </ComboboxContent>
            </Combobox>

            {hasDiverged && onUpdateSavedFilter && (
              <Button
                variant="outline"
                size="sm"
                className="shrink-0 text-muted-foreground hover:text-primary"
                onClick={() =>
                  onUpdateSavedFilter(selectedSavedFilter!.id)
                }
                title="Update saved filter with current filters"
              >
                <HugeiconsIcon icon={FloppyDiskIcon} className="size-4" />
              </Button>
            )}

            {onDeleteSavedFilter && (
              <Button
                variant="outline"
                size="sm"
                className="shrink-0 text-muted-foreground hover:text-destructive"
                disabled={!selectedSavedFilter}
                onClick={() => {
                  if (selectedSavedFilter) {
                    onDeleteSavedFilter(selectedSavedFilter.id)
                    setSelectedSavedFilterId(null)
                    setActiveSource(null)
                  }
                }}
              >
                <HugeiconsIcon icon={Delete02Icon} className="size-4" />
              </Button>
            )}
          </div>
        </div>
      )}

      {/* Edit filter modal (supports all filter types) */}
      {editingFilter && (
        <FilterEditModal
          isOpen={editModalOpen}
          onClose={() => {
            setEditModalOpen(false)
            setEditingFilter(null)
          }}
          onSubmit={handleUpdateFilter}
          filter={editingFilter}
          filterOptions={filterOptions}
        />
      )}
    </div>
  )
}

// Export utility function to convert state to QueryKit string
export { toQueryKitString }
