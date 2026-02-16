import {
  ArrowLeft02Icon,
  Loading03Icon,
  Upload04Icon,
  CheckmarkCircle02Icon,
  AlertCircleIcon,
  ImageIcon,
  StarIcon,
} from "@hugeicons/core-free-icons";
import { HugeiconsIcon } from "@hugeicons/react";
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useCallback, useMemo, useRef, useState } from "react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { usePreviewCmtImport, useImportCmtRecipes } from "@/domain/recipes/apis/recipe-mutations";
import type { CmtImportPreviewDto, CmtImportResultDto } from "@/domain/recipes/types";

export const Route = createFileRoute("/recipes/import-cmt")({
  component: ImportCmtPage,
});

function ImportCmtPage() {
  const navigate = useNavigate();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<CmtImportPreviewDto | null>(null);
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [importRatings, setImportRatings] = useState(true);
  const [result, setResult] = useState<CmtImportResultDto | null>(null);

  const previewMutation = usePreviewCmtImport();
  const importMutation = useImportCmtRecipes();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0];
    if (f) {
      setFile(f);
      setPreview(null);
      setResult(null);
    }
  };

  const handleUpload = () => {
    if (!file) return;
    previewMutation.mutate(file, {
      onSuccess: (data) => {
        setPreview(data);
        // Select all non-duplicate recipes by default
        const initialSelected = new Set(
          data.recipes
            .filter((r) => !r.isDuplicate)
            .map((r) => r.index)
        );
        setSelected(initialSelected);
      },
    });
  };

  const handleImport = () => {
    if (!file || selected.size === 0) return;
    importMutation.mutate(
      {
        file,
        selectedIndices: Array.from(selected),
        importRatings,
      },
      {
        onSuccess: (data) => {
          setResult(data);
        },
      }
    );
  };

  const toggleRecipe = useCallback((index: number) => {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(index)) {
        next.delete(index);
      } else {
        next.add(index);
      }
      return next;
    });
  }, []);

  const selectAll = useCallback(() => {
    if (!preview) return;
    setSelected(new Set(preview.recipes.map((r) => r.index)));
  }, [preview]);

  const deselectAll = useCallback(() => {
    setSelected(new Set());
  }, []);

  const selectedCount = selected.size;

  const recipesWithRatings = useMemo(
    () => preview?.recipes.filter((r) => r.rating !== null).length ?? 0,
    [preview]
  );

  // -- Render --

  if (result) {
    return (
      <div className="space-y-6">
        <Header onBack={() => navigate({ to: "/recipes" })} />
        <div className="mx-auto max-w-3xl">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <HugeiconsIcon
                  icon={CheckmarkCircle02Icon}
                  className="h-5 w-5 text-green-600"
                />
                Import Complete
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-3 gap-4 text-center">
                <div>
                  <div className="text-2xl font-bold">{result.importedCount}</div>
                  <div className="text-sm text-muted-foreground">Imported</div>
                </div>
                <div>
                  <div className="text-2xl font-bold">{result.skippedCount}</div>
                  <div className="text-sm text-muted-foreground">Skipped</div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-destructive">
                    {result.errorCount}
                  </div>
                  <div className="text-sm text-muted-foreground">Errors</div>
                </div>
              </div>

              {result.errors.length > 0 && (
                <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4">
                  <p className="mb-2 font-medium text-destructive">Errors:</p>
                  <ul className="space-y-1 text-sm text-destructive">
                    {result.errors.map((err, i) => (
                      <li key={i}>{err}</li>
                    ))}
                  </ul>
                </div>
              )}

              <div className="flex gap-3">
                <Button onClick={() => navigate({ to: "/recipes" })}>
                  View Recipes
                </Button>
                <Button
                  variant="outline"
                  onClick={() => {
                    setFile(null);
                    setPreview(null);
                    setResult(null);
                    setSelected(new Set());
                  }}
                >
                  Import More
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <Header onBack={() => navigate({ to: "/recipes" })} />

      <div className="mx-auto max-w-5xl space-y-6">
        {/* Upload Card */}
        <Card>
          <CardHeader>
            <CardTitle>Upload ZIP File</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center gap-3">
              <input
                ref={fileInputRef}
                type="file"
                accept=".zip"
                className="hidden"
                onChange={handleFileChange}
              />
              <Button
                variant="outline"
                onClick={() => fileInputRef.current?.click()}
              >
                <HugeiconsIcon icon={Upload04Icon} className="mr-2 h-4 w-4" />
                {file ? file.name : "Choose ZIP file"}
              </Button>
              <Button
                onClick={handleUpload}
                disabled={!file || previewMutation.isPending}
              >
                {previewMutation.isPending ? (
                  <>
                    <HugeiconsIcon
                      icon={Loading03Icon}
                      className="mr-2 h-4 w-4 animate-spin"
                    />
                    Parsing...
                  </>
                ) : (
                  "Upload & Preview"
                )}
              </Button>
            </div>

            {previewMutation.isError && (
              <p className="mt-3 text-sm font-medium text-destructive">
                {previewMutation.error instanceof Error
                  ? previewMutation.error.message
                  : "Failed to parse ZIP file."}
              </p>
            )}
          </CardContent>
        </Card>

        {/* Preview */}
        {preview && (
          <>
            {/* Summary & Controls */}
            <Card>
              <CardContent className="pt-6">
                <div className="flex flex-wrap items-center justify-between gap-4">
                  <div className="space-y-1">
                    <p className="text-sm font-medium">
                      Found {preview.totalCount} recipes
                      {preview.duplicateCount > 0 && (
                        <span className="text-muted-foreground">
                          {" "}({preview.duplicateCount} duplicates)
                        </span>
                      )}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {selectedCount} selected for import
                    </p>
                  </div>

                  <div className="flex flex-wrap items-center gap-3">
                    {recipesWithRatings > 0 && (
                      <Checkbox
                        isSelected={importRatings}
                        onChange={setImportRatings}
                      >
                        Import ratings ({recipesWithRatings} rated)
                      </Checkbox>
                    )}

                    <div className="flex gap-2">
                      <Button variant="outline" size="sm" onClick={selectAll}>
                        Select All
                      </Button>
                      <Button variant="outline" size="sm" onClick={deselectAll}>
                        Deselect All
                      </Button>
                    </div>
                  </div>
                </div>

                {importRatings && recipesWithRatings > 0 && (
                  <div className="mt-3 rounded-md border bg-muted/50 p-3 text-xs text-muted-foreground">
                    <span className="font-medium">Rating mapping:</span>{" "}
                    5 = Loved It, 4 = Liked It, 3 = It Was Ok, 2 = Not Great, 1 = Hated It
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Recipe Table */}
            <Card>
              <CardContent className="p-0">
                <div className="max-h-[60vh] overflow-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead className="w-10" />
                        <TableHead>Title</TableHead>
                        <TableHead className="w-20 text-center">Srv.</TableHead>
                        <TableHead className="w-20 text-center">Ingr.</TableHead>
                        <TableHead className="w-20 text-center">Rating</TableHead>
                        <TableHead className="w-16 text-center">Img</TableHead>
                        <TableHead className="w-28" />
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {preview.recipes.map((recipe) => (
                        <TableRow
                          key={recipe.index}
                          className={recipe.isDuplicate ? "opacity-60" : undefined}
                        >
                          <TableCell>
                            <Checkbox
                              isSelected={selected.has(recipe.index)}
                              onChange={() => toggleRecipe(recipe.index)}
                              aria-label={`Select ${recipe.title}`}
                            />
                          </TableCell>
                          <TableCell>
                            <div className="max-w-md">
                              <span className="font-medium">{recipe.title}</span>
                              {recipe.source && (
                                <span className="ml-2 text-xs text-muted-foreground truncate">
                                  {truncateSource(recipe.source)}
                                </span>
                              )}
                            </div>
                          </TableCell>
                          <TableCell className="text-center text-sm">
                            {recipe.servings ?? "-"}
                          </TableCell>
                          <TableCell className="text-center text-sm">
                            {recipe.ingredientCount}
                          </TableCell>
                          <TableCell className="text-center text-sm">
                            {recipe.rating ? (
                              <span className="inline-flex items-center gap-1">
                                <HugeiconsIcon icon={StarIcon} className="h-3 w-3" />
                                {recipe.rating}
                              </span>
                            ) : (
                              "-"
                            )}
                          </TableCell>
                          <TableCell className="text-center">
                            {recipe.hasImage && (
                              <HugeiconsIcon
                                icon={ImageIcon}
                                className="mx-auto h-4 w-4 text-muted-foreground"
                              />
                            )}
                          </TableCell>
                          <TableCell>
                            {recipe.isDuplicate && (
                              <Badge variant="destructive">Duplicate</Badge>
                            )}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>

            {/* Import Button */}
            <div className="flex items-center justify-between">
              <p className="text-sm text-muted-foreground">
                {selectedCount} of {preview.totalCount} recipes selected
              </p>
              <Button
                onClick={handleImport}
                disabled={selectedCount === 0 || importMutation.isPending}
                size="lg"
              >
                {importMutation.isPending ? (
                  <>
                    <HugeiconsIcon
                      icon={Loading03Icon}
                      className="mr-2 h-4 w-4 animate-spin"
                    />
                    Importing...
                  </>
                ) : (
                  `Import Selected (${selectedCount})`
                )}
              </Button>
            </div>

            {importMutation.isError && (
              <div className="rounded-lg border border-destructive/20 bg-destructive/10 p-4 text-destructive">
                <div className="flex items-center gap-2">
                  <HugeiconsIcon icon={AlertCircleIcon} className="h-4 w-4" />
                  <p className="font-medium">Import failed</p>
                </div>
                <p className="mt-1 text-sm">
                  {importMutation.error instanceof Error
                    ? importMutation.error.message
                    : "An unexpected error occurred"}
                </p>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}

function Header({ onBack }: { onBack: () => void }) {
  return (
    <div className="flex items-center gap-4">
      <Button variant="ghost" size="icon" onClick={onBack}>
        <HugeiconsIcon icon={ArrowLeft02Icon} className="h-5 w-5" />
      </Button>
      <div>
        <h1 className="text-3xl font-bold tracking-tight">
          Import from Copy Me That
        </h1>
        <p className="text-muted-foreground">
          Upload a Copy Me That ZIP export to import your recipes
        </p>
      </div>
    </div>
  );
}

function truncateSource(source: string): string {
  try {
    const url = new URL(source);
    return url.hostname;
  } catch {
    return source.length > 40 ? source.slice(0, 40) + "..." : source;
  }
}
