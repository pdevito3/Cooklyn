import { useState, useCallback } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { ArrowLeft02Icon, Delete02Icon, DragDropIcon } from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import {
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
  arrayMove,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

import { useStore, useUpdateStore, useDeleteStore, useUpdateStoreAisles } from '@/domain/stores'
import type { StoreAisleForUpdateDto } from '@/domain/stores'
import { DEFAULT_STORE_AISLES } from '@/domain/stores/constants'
import { useStoreSections, createStoreSection } from '@/domain/store-sections'
import type { StoreSectionDto } from '@/domain/store-sections'
import { useMyDefaultStore, useUpdateMyDefaultStore } from '@/domain/users'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Combobox,
  ComboboxInput,
  ComboboxContent,
  ComboboxItem,
  ComboboxItemIndicator,
} from '@/components/ui/combobox'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'

export const Route = createFileRoute('/stores/$id/')({
  component: StoreDetailPage,
})

function SortableAisleRow({
  aisle,
  index,
  sections,
  onSectionChange,
  onCustomNameChange,
  onRemove,
}: {
  aisle: StoreAisleForUpdateDto
  index: number
  sections: { id: string; name: string }[]
  onSectionChange: (index: number, value: string) => void
  onCustomNameChange: (index: number, value: string) => void
  onRemove: (index: number) => void
}) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: `aisle-${index}` })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  }

  return (
    <div ref={setNodeRef} style={style} className="flex items-center gap-3">
      <button
        type="button"
        className="cursor-grab touch-none text-muted-foreground hover:text-foreground"
        {...attributes}
        {...listeners}
      >
        <HugeiconsIcon icon={DragDropIcon} className="h-4 w-4" />
      </button>
      <span className="text-sm font-medium text-muted-foreground w-6">{index + 1}</span>
      <Combobox
        items={sections}
        value={sections.find((s) => s.id === aisle.storeSectionId) ?? null}
        onValueChange={(section: { id: string; name: string } | null) => {
          if (section == null) return
          onSectionChange(index, section.id)
        }}
        itemToStringLabel={(section) => section?.name ?? ''}
      >
        <ComboboxInput placeholder="Select section" className="w-48" autoFocus />
        <ComboboxContent emptyMessage="No sections found.">
          {(section: { id: string; name: string }) => (
            <ComboboxItem key={section.id} value={section}>
              <span className="flex-1">{section.name}</span>
              <ComboboxItemIndicator />
            </ComboboxItem>
          )}
        </ComboboxContent>
      </Combobox>
      <Input
        className="flex-1"
        placeholder="Custom name (optional)"
        value={aisle.customName ?? ''}
        onChange={(e) => onCustomNameChange(index, e.target.value)}
      />
      <Button variant="ghost" size="icon" onClick={() => onRemove(index)}>
        <HugeiconsIcon icon={Delete02Icon} className="h-4 w-4" />
      </Button>
    </div>
  )
}

function StoreDetailPage() {
  const { id } = Route.useParams()
  const navigate = useNavigate()
  const { data: store, isLoading } = useStore(id)
  const { data: sectionsData, refetch: refetchSections } = useStoreSections({ pageSize: 100 })
  const updateStore = useUpdateStore()
  const deleteStoreMutation = useDeleteStore()
  const updateAisles = useUpdateStoreAisles()
  const { data: defaultStoreId } = useMyDefaultStore()
  const updateDefaultStore = useUpdateMyDefaultStore()
  const [deleteOpen, setDeleteOpen] = useState(false)
  const [editName, setEditName] = useState('')
  const [editAddress, setEditAddress] = useState('')
  const [isEditing, setIsEditing] = useState(false)
  const [aisles, setAisles] = useState<StoreAisleForUpdateDto[]>([])
  const [aislesEditing, setAislesEditing] = useState(false)
  const [loadingDefaults, setLoadingDefaults] = useState(false)

  const sectionItems = sectionsData?.items
  const sections = sectionItems ?? []

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const startEditing = () => {
    if (!store) return
    setEditName(store.name)
    setEditAddress(store.address ?? '')
    setIsEditing(true)
  }

  const saveEdit = () => {
    updateStore.mutate(
      { id, dto: { name: editName, address: editAddress || null } },
      { onSuccess: () => setIsEditing(false) }
    )
  }

  const startAisleEditing = () => {
    if (!store) return
    setAisles(
      store.storeAisles.map((a) => ({
        storeSectionId: a.storeSectionId,
        sortOrder: a.sortOrder,
        customName: a.customName,
      }))
    )
    setAislesEditing(true)
  }

  const addAisle = () => {
    setAisles((prev) => [
      ...prev,
      { storeSectionId: '', sortOrder: prev.length, customName: null },
    ])
  }

  const removeAisle = (index: number) => {
    setAisles((prev) => prev.filter((_, i) => i !== index).map((a, i) => ({ ...a, sortOrder: i })))
  }

  const loadDefaultAisles = useCallback(async () => {
    setLoadingDefaults(true)
    try {
      // Build a map of existing sections by name
      const currentSections = sectionItems ?? []
      const sectionsByName = new Map(currentSections.map((s) => [s.name, s]))

      // Create any missing sections
      const createdSections: StoreSectionDto[] = []
      for (const aisleName of DEFAULT_STORE_AISLES) {
        if (!sectionsByName.has(aisleName)) {
          const created = await createStoreSection({ name: aisleName })
          createdSections.push(created)
          sectionsByName.set(aisleName, created)
        }
      }

      // Refetch sections if we created any
      if (createdSections.length > 0) {
        await refetchSections()
      }

      // Now add all defaults as aisles
      const existingSectionIds = new Set(aisles.map((a) => a.storeSectionId))
      const newAisles: StoreAisleForUpdateDto[] = [...aisles]

      for (const aisleName of DEFAULT_STORE_AISLES) {
        const section = sectionsByName.get(aisleName)
        if (section && !existingSectionIds.has(section.id)) {
          newAisles.push({
            storeSectionId: section.id,
            sortOrder: newAisles.length,
            customName: null,
          })
          existingSectionIds.add(section.id)
        }
      }

      setAisles(newAisles.map((a, i) => ({ ...a, sortOrder: i })))
    } finally {
      setLoadingDefaults(false)
    }
  }, [sectionItems, aisles, refetchSections])

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return

    const oldIndex = aisles.findIndex((_, i) => `aisle-${i}` === active.id)
    const newIndex = aisles.findIndex((_, i) => `aisle-${i}` === over.id)

    if (oldIndex !== -1 && newIndex !== -1) {
      setAisles((prev) =>
        arrayMove(prev, oldIndex, newIndex).map((a, i) => ({ ...a, sortOrder: i }))
      )
    }
  }

  const handleSectionChange = (index: number, value: string) => {
    setAisles((prev) =>
      prev.map((a, i) => (i === index ? { ...a, storeSectionId: value } : a))
    )
  }

  const handleCustomNameChange = (index: number, value: string) => {
    setAisles((prev) =>
      prev.map((a, i) =>
        i === index ? { ...a, customName: value || null } : a
      )
    )
  }

  const saveAisles = () => {
    updateAisles.mutate(
      { id, aisles },
      { onSuccess: () => setAislesEditing(false) }
    )
  }

  const confirmDelete = () => {
    deleteStoreMutation.mutate(id, {
      onSuccess: () => navigate({ to: '/stores' }),
    })
  }

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    )
  }

  if (!store) {
    return <p className="text-muted-foreground">Store not found.</p>
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={() => navigate({ to: '/stores' })}>
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
        </Button>
        <div className="flex-1">
          <h1 className="text-3xl font-bold tracking-tight">{store.name}</h1>
          {store.address && <p className="text-muted-foreground">{store.address}</p>}
        </div>
        {defaultStoreId === id ? (
          <Button variant="outline" disabled>Default Store</Button>
        ) : (
          <Button
            variant="outline"
            onClick={() => updateDefaultStore.mutate({ storeId: id })}
            disabled={updateDefaultStore.isPending}
          >
            Set as Default
          </Button>
        )}
        <Button variant="outline" onClick={startEditing}>Edit</Button>
        <Button variant="destructive" onClick={() => setDeleteOpen(true)}>Delete</Button>
      </div>

      {isEditing && (
        <Card>
          <CardHeader>
            <CardTitle>Edit Store</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label>Name</Label>
              <Input value={editName} onChange={(e) => setEditName(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>Address</Label>
              <Input value={editAddress} onChange={(e) => setEditAddress(e.target.value)} />
            </div>
            <div className="flex gap-2">
              <Button onClick={saveEdit} disabled={updateStore.isPending}>Save</Button>
              <Button variant="outline" onClick={() => setIsEditing(false)}>Cancel</Button>
            </div>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Aisle Ordering</CardTitle>
          {!aislesEditing && <Button variant="outline" onClick={startAisleEditing}>Edit Aisles</Button>}
        </CardHeader>
        <CardContent>
          {!aislesEditing ? (
            store.storeAisles.length === 0 ? (
              <p className="text-sm text-muted-foreground">No aisles configured.</p>
            ) : (
              <div className="space-y-2">
                {[...store.storeAisles]
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((aisle) => {
                    const section = sections.find((s) => s.id === aisle.storeSectionId)
                    return (
                      <div key={aisle.id} className="flex items-center gap-3 rounded-md border p-3">
                        <span className="text-sm font-medium text-muted-foreground w-6">{aisle.sortOrder + 1}</span>
                        <span className="font-medium">{aisle.customName ?? section?.name ?? 'Unknown'}</span>
                        {aisle.customName && section && (
                          <span className="text-sm text-muted-foreground">({section.name})</span>
                        )}
                      </div>
                    )
                  })}
              </div>
            )
          ) : (
            <div className="space-y-3">
              <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragEnd={handleDragEnd}
              >
                <SortableContext
                  items={aisles.map((_, i) => `aisle-${i}`)}
                  strategy={verticalListSortingStrategy}
                >
                  {aisles.map((aisle, index) => (
                    <SortableAisleRow
                      key={`aisle-${index}`}
                      aisle={aisle}
                      index={index}
                      sections={sections}
                      onSectionChange={handleSectionChange}
                      onCustomNameChange={handleCustomNameChange}
                      onRemove={removeAisle}
                    />
                  ))}
                </SortableContext>
              </DndContext>
              <div className="flex gap-2">
                <Button variant="outline" onClick={addAisle}>Add Aisle</Button>
                <Button variant="outline" onClick={loadDefaultAisles} disabled={loadingDefaults}>
                  {loadingDefaults ? 'Loading...' : 'Load Default Aisles'}
                </Button>
                <Button onClick={saveAisles} disabled={updateAisles.isPending}>Save</Button>
                <Button variant="outline" onClick={() => setAislesEditing(false)}>Cancel</Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Store</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete "{store.name}"? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteStoreMutation.isPending ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
