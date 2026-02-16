import {
  Add01Icon,
  ArrowLeft02Icon,
  Delete02Icon,
  TextIcon,
} from "@hugeicons/core-free-icons";
import { HugeiconsIcon } from "@hugeicons/react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useEffect, useRef, useState } from "react";
import { useHotkeys } from "react-hotkeys-hook";

import { IngredientEditor } from "@/components/ingredient-editor";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Combobox,
  ComboboxInput,
  ComboboxContent,
  ComboboxItem,
  ComboboxItemIndicator,
} from "@/components/ui/combobox";
import { Input } from "@/components/ui/input";
import { Kbd } from "@/components/ui/kbd";
import { Label } from "@/components/ui/label";
import { Skeleton } from "@/components/ui/skeleton";
import type { ItemCollectionItemForCreationDto } from "@/domain/item-collections/types";
import { useItemCollection } from "@/domain/item-collections/apis/get-item-collection";
import {
  useDeleteItemCollection,
  useUpdateItemCollection,
  useUpdateItemCollectionItems,
} from "@/domain/item-collections/apis/item-collection-mutations";
import { ingredientsToCollectionItems } from "@/domain/item-collections/utils/ingredient-to-collection-item";
import type { IngredientForCreationDto } from "@/domain/recipes/types";
import { useStoreSections } from "@/domain/store-sections/apis/get-store-sections";
import type { StoreSectionDto } from "@/domain/store-sections/types";

export const Route = createFileRoute("/collections/$id/")({
  component: CollectionDetailPage,
});

function CollectionDetailPage() {
  const { id } = Route.useParams();
  const navigate = useNavigate();
  const { data: collection, isLoading } = useItemCollection(id);
  const { data: sectionsData } = useStoreSections({ pageSize: 100 });
  const updateCollection = useUpdateItemCollection();
  const deleteCollectionMutation = useDeleteItemCollection();
  const updateItems = useUpdateItemCollectionItems();

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editName, setEditName] = useState("");
  const [isEditingName, setIsEditingName] = useState(false);
  const [items, setItems] = useState<ItemCollectionItemForCreationDto[]>([]);
  const [itemsSynced, setItemsSynced] = useState(false);
  const [showTextInput, setShowTextInput] = useState(false);
  const [textIngredients, setTextIngredients] = useState<
    IngredientForCreationDto[]
  >([]);

  const sections = sectionsData?.items ?? [];
  const lastItemNameRef = useRef<HTMLInputElement>(null);

  useHotkeys("e", () => {
    if (collection && !isEditingName) startEditingName();
  });
  useHotkeys("delete", () => {
    if (collection && !isEditingName) setDeleteOpen(true);
  });

  // Sync items from collection data when it loads
  useEffect(() => {
    if (collection && !itemsSynced) {
      setItems(
        collection.items.map((item) => ({
          name: item.name,
          quantity: item.quantity,
          unit: item.unit,
          storeSectionId: item.storeSectionId,
          sortOrder: item.sortOrder,
        })),
      );
      setItemsSynced(true);
    }
  }, [collection, itemsSynced]);

  const startEditingName = () => {
    if (!collection) return;
    setEditName(collection.name);
    setIsEditingName(true);
  };

  const saveNameEdit = () => {
    updateCollection.mutate(
      { id, dto: { name: editName } },
      { onSuccess: () => setIsEditingName(false) },
    );
  };

  const addItem = () => {
    setItems((prev) => [
      ...prev,
      {
        name: "",
        quantity: null,
        unit: null,
        storeSectionId: null,
        sortOrder: prev.length,
      },
    ]);
    setTimeout(() => lastItemNameRef.current?.focus(), 0);
  };

  const removeItem = (index: number) => {
    setItems((prev) =>
      prev
        .filter((_, i) => i !== index)
        .map((item, i) => ({ ...item, sortOrder: i })),
    );
  };

  const addTextItems = () => {
    if (textIngredients.length === 0) return;
    const newItems = ingredientsToCollectionItems(
      textIngredients,
      items.length,
    );
    setItems((prev) => [...prev, ...newItems]);
    setShowTextInput(false);
    setTextIngredients([]);
  };

  const saveItems = () => {
    updateItems.mutate({ id, items });
  };

  const confirmDelete = () => {
    deleteCollectionMutation.mutate(id, {
      onSuccess: () => navigate({ to: "/collections" }),
    });
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (!collection) {
    return <p className="text-muted-foreground">Collection not found.</p>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate({ to: "/collections" })}
        >
          <HugeiconsIcon icon={ArrowLeft02Icon} className="h-4 w-4" />
        </Button>
        <div className="flex-1">
          <h1 className="text-3xl font-bold tracking-tight">
            {collection.name}
          </h1>
          <p className="text-muted-foreground">
            {collection.items.length} item
            {collection.items.length !== 1 ? "s" : ""}
          </p>
        </div>
        <Button variant="outline" onClick={startEditingName}>
          Edit
          <Kbd>E</Kbd>
        </Button>
        <Button variant="destructive" onClick={() => setDeleteOpen(true)}>
          Delete
          <Kbd>⌫</Kbd>
        </Button>
      </div>

      {isEditingName && (
        <Card>
          <CardHeader>
            <CardTitle>Edit Collection</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label>Name</Label>
              <Input
                value={editName}
                onChange={(e) => setEditName(e.target.value)}
              />
            </div>
            <div className="flex gap-2">
              <Button
                onClick={saveNameEdit}
                disabled={updateCollection.isPending}
              >
                Save
              </Button>
              <Button
                variant="outline"
                onClick={() => setIsEditingName(false)}
              >
                Cancel
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Items</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {items.map((item, index) => (
              <div key={index} className="flex items-center gap-3">
                <Input
                  ref={
                    index === items.length - 1 ? lastItemNameRef : undefined
                  }
                  className="flex-1"
                  placeholder="Item name"
                  value={item.name}
                  onChange={(e) => {
                    setItems((prev) =>
                      prev.map((it, i) =>
                        i === index ? { ...it, name: e.target.value } : it,
                      ),
                    );
                  }}
                />
                <Input
                  className="w-20"
                  placeholder="Qty"
                  type="number"
                  value={item.quantity ?? ""}
                  onChange={(e) => {
                    const val = e.target.value ? Number(e.target.value) : null;
                    setItems((prev) =>
                      prev.map((it, i) =>
                        i === index ? { ...it, quantity: val } : it,
                      ),
                    );
                  }}
                />
                <Input
                  className="w-24"
                  placeholder="Unit"
                  value={item.unit ?? ""}
                  onChange={(e) => {
                    setItems((prev) =>
                      prev.map((it, i) =>
                        i === index
                          ? { ...it, unit: e.target.value || null }
                          : it,
                      ),
                    );
                  }}
                />
                <Combobox
                  items={sections}
                  value={
                    sections.find((s) => s.id === item.storeSectionId) ?? null
                  }
                  onValueChange={(section: StoreSectionDto | null) => {
                    setItems((prev) =>
                      prev.map((it, i) =>
                        i === index
                          ? { ...it, storeSectionId: section?.id ?? null }
                          : it,
                      ),
                    );
                  }}
                  itemToStringLabel={(section) => section?.name ?? ""}
                >
                  <ComboboxInput placeholder="Section" className="w-40" />
                  <ComboboxContent emptyMessage="No sections found.">
                    {(section: StoreSectionDto) => (
                      <ComboboxItem key={section.id} value={section}>
                        <span className="flex-1">{section.name}</span>
                        <ComboboxItemIndicator />
                      </ComboboxItem>
                    )}
                  </ComboboxContent>
                </Combobox>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => removeItem(index)}
                >
                  <HugeiconsIcon icon={Delete02Icon} className="h-4 w-4" />
                </Button>
              </div>
            ))}

            {showTextInput && (
              <div className="space-y-3 rounded-md border p-4">
                <p className="text-sm text-muted-foreground">
                  Enter items one per line (e.g. "2 cups flour", "1 lb chicken
                  breast")
                </p>
                <IngredientEditor
                  value={textIngredients}
                  onChange={setTextIngredients}
                />
                <div className="flex gap-2">
                  <Button
                    onClick={addTextItems}
                    disabled={textIngredients.length === 0}
                  >
                    Add {textIngredients.length} Item
                    {textIngredients.length !== 1 ? "s" : ""}
                  </Button>
                  <Button
                    variant="outline"
                    onClick={() => {
                      setShowTextInput(false);
                      setTextIngredients([]);
                    }}
                  >
                    Cancel
                  </Button>
                </div>
              </div>
            )}

            <div className="flex gap-2">
              <Button variant="outline" onClick={addItem}>
                <HugeiconsIcon icon={Add01Icon} className="mr-2 h-4 w-4" />
                Add Item
              </Button>
              {!showTextInput && (
                <Button
                  variant="outline"
                  onClick={() => {
                    setTextIngredients([]);
                    setShowTextInput(true);
                  }}
                >
                  <HugeiconsIcon icon={TextIcon} className="mr-2 h-4 w-4" />
                  Add via Text
                </Button>
              )}
              <Button onClick={saveItems} disabled={updateItems.isPending}>
                {updateItems.isPending ? "Saving..." : "Save"}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Collection</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete "{collection.name}"? This action
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteCollectionMutation.isPending ? "Deleting..." : "Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
