# Recipe Ingredients Backend Design

## Overview

Backend support for managing ingredients on a recipe with structured data storage, flexible input modes (free text, website import), and optional amount/unit fields.

## Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Data model | Per-recipe only | No shared ingredient catalog. Each recipe owns its ingredient list. Cross-recipe search uses text matching. |
| Parsing | Frontend + backend | Frontend parses on keystroke for instant UX. Backend re-parses/validates on save for data integrity. |
| Raw text | Always preserved | `RawText` field stores original input alongside parsed fields. Handles edge cases where parsing fails gracefully. |
| Ordering | Global SortOrder | Single `SortOrder` int across all ingredients in a recipe. Group display order derived from first ingredient in each group. |
| Grouping | Optional GroupName | Nullable string field. Ungrouped ingredients have `GroupName = null`. Groups are ordered, ungrouped items appear based on their SortOrder position. |
| Units | SmartEnum + Custom sentinel | Predefined known units with `Custom` sentinel member for freeform fallback. `CustomUnit` string holds text when `Unit == Custom`. |
| Amounts | Fractions and decimals | `Amount` (decimal?) for math/scaling, `AmountText` (string?) preserves display like "1 1/2". |
| Website import | Separate endpoint | Dedicated `import-ingredients` endpoint, decoupled from the text parsing endpoint. Can be deferred. |

## Data Model

### RecipeIngredient Entity

Child entity owned by Recipe, following existing patterns (`RecipeFlagEntry`, `RecipeTag`).

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | `Guid` | Yes | From `BaseEntity` |
| `RecipeId` | `Guid` | Yes | FK to Recipe |
| `RawText` | `string` | Yes | Original text the user typed, always preserved |
| `Name` | `string?` | No | Parsed ingredient name ("all-purpose flour") |
| `Amount` | `decimal?` | No | Parsed numeric value (1.5) for math/scaling |
| `AmountText` | `string?` | No | Display text preserving fractions ("1 1/2") |
| `Unit` | `IngredientUnit?` | No | SmartEnum value object (known type OR Custom sentinel) |
| `CustomUnit` | `string?` | No | Populated only when `Unit == IngredientUnit.Custom` |
| `GroupName` | `string?` | No | Optional group name ("For the Biscuits"), null = ungrouped |
| `SortOrder` | `int` | Yes | Global order across entire recipe |

### IngredientUnit SmartEnum Value Object

Follows the `RecipeFlag` pattern. Approximately 30 known units organized by category, plus a `Custom` sentinel.

Each unit has:
- `Abbreviation` — short display ("cup", "tbsp", "tsp")
- `PluralName` — plural form ("cups", "tablespoons")
- `ParseAliases` — array of strings the parser recognizes (["c", "cup", "cups"])

#### Known Units

**Volume:** Cup, Tablespoon, Teaspoon, FluidOunce, Milliliter, Liter, Pint, Quart, Gallon

**Weight:** Ounce, Pound, Gram, Kilogram

**Count/Descriptive:** Piece, Whole, Slice, Clove, Pinch, Dash, Can, Bunch, Sprig, Stick

**Sentinel:** Custom (freeform fallback — `CustomUnit` string holds actual text)

#### Unit Resolution Logic

- `Unit = IngredientUnit.Cup` + `CustomUnit = null` — known unit
- `Unit = IngredientUnit.Custom` + `CustomUnit = "handful"` — freeform unit
- `Unit = null` + `CustomUnit = null` — no unit at all ("salt and pepper to taste")

### Relationship to Recipe Entity

```csharp
// In Recipe.cs — follows Flags/RecipeTags collection pattern
public IReadOnlyCollection<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();
private readonly List<RecipeIngredient> _ingredients = [];

public void SetIngredients(IEnumerable<RecipeIngredient> ingredients)
{
    _ingredients.Clear();
    _ingredients.AddRange(ingredients);
    QueueDomainEvent(new RecipeUpdated(Id));
}
```

## API Endpoints

### Replace All Ingredients

Matches existing tags/flags "replace all" pattern.

```
PUT /api/v1/recipes/{id}/ingredients
```

**Request body:**
```json
[
  {
    "rawText": "2 cups all-purpose flour",
    "name": "all-purpose flour",
    "amount": 2.0,
    "amountText": "2",
    "unit": "Cup",
    "customUnit": null,
    "groupName": "For the Biscuits",
    "sortOrder": 0
  },
  {
    "rawText": "1/2 tsp salt",
    "name": "salt",
    "amount": 0.5,
    "amountText": "1/2",
    "unit": "Teaspoon",
    "customUnit": null,
    "groupName": "For the Biscuits",
    "sortOrder": 1
  }
]
```

**Response:** Updated `RecipeDto` with full ingredient list.

### Parse Raw Text

Backend parsing/validation endpoint.

```
POST /api/v1/recipes/parse-ingredients
```

**Request body:**
```json
{
  "text": "2 cups flour\n1/2 tsp salt\n3 large eggs\nsalt and pepper to taste"
}
```

**Response:**
```json
[
  {
    "rawText": "2 cups flour",
    "name": "flour",
    "amount": 2.0,
    "amountText": "2",
    "unit": "Cup",
    "customUnit": null,
    "sortOrder": 0
  },
  {
    "rawText": "1/2 tsp salt",
    "name": "salt",
    "amount": 0.5,
    "amountText": "1/2",
    "unit": "Teaspoon",
    "customUnit": null,
    "sortOrder": 1
  },
  {
    "rawText": "3 large eggs",
    "name": "large eggs",
    "amount": 3.0,
    "amountText": "3",
    "unit": null,
    "customUnit": null,
    "sortOrder": 2
  },
  {
    "rawText": "salt and pepper to taste",
    "name": "salt and pepper to taste",
    "amount": null,
    "amountText": null,
    "unit": null,
    "customUnit": null,
    "sortOrder": 3
  }
]
```

### Import From Website (Deferred)

Separate endpoint for scraping ingredients from a URL.

```
POST /api/v1/recipes/import-ingredients
```

**Request body:**
```json
{
  "url": "https://example.com/recipe/chocolate-cake"
}
```

**Response:** Same structure as parse-ingredients response.

## Parsing Strategy

### Parser Logic

1. **Split** input by newlines into individual ingredient lines
2. **Extract amount** — match leading number patterns:
   - Integer: `3`
   - Decimal: `1.5`
   - Fraction: `1/2`
   - Mixed number: `1 1/2`
   - Convert to `decimal` for `Amount`, preserve original as `AmountText`
3. **Match unit** — check next word(s) against `IngredientUnit.ParseAliases`:
   - If match found: set `Unit` to the matched SmartEnum value
   - If no match: leave `Unit` as null (the word is likely part of the name)
4. **Remainder** — everything left after amount and unit extraction becomes `Name`
5. **Edge cases:**
   - "a handful of basil" → `Amount=null`, `Unit=Custom`, `CustomUnit="handful"`, `Name="basil"`
   - "salt and pepper to taste" → `Amount=null`, `Unit=null`, `Name="salt and pepper to taste"`
   - "3 large eggs" → `Amount=3`, `Unit=null`, `Name="large eggs"`

### Dual Parsing (Frontend + Backend)

- **Frontend** parses on keystroke for instant structured preview. User sees parsed amount/unit/name update in real time as they type.
- **Backend** re-parses on save. If the frontend sent parsed fields, backend validates them. If only raw text is sent, backend parses from scratch.
- Parsing logic should be consistent between frontend and backend but the backend is the source of truth.

## Data Flow Examples

### Free Text Input

```
User types:        "1 1/2 cups all-purpose flour"
  RawText:         "1 1/2 cups all-purpose flour"
  Name:            "all-purpose flour"
  Amount:          1.5
  AmountText:      "1 1/2"
  Unit:            IngredientUnit.Cup
  CustomUnit:      null
```

### Custom Unit

```
User types:        "a handful of basil"
  RawText:         "a handful of basil"
  Name:            "basil"
  Amount:          null
  AmountText:      null
  Unit:            IngredientUnit.Custom
  CustomUnit:      "handful"
```

### No Amount, No Unit

```
User types:        "salt and pepper to taste"
  RawText:         "salt and pepper to taste"
  Name:            "salt and pepper to taste"
  Amount:          null
  AmountText:      null
  Unit:            null
  CustomUnit:      null
```

### Grouped Ingredients

```
Recipe: "Biscuits and Gravy"

SortOrder 0: [Group: "For the Biscuits"] 2 cups flour
SortOrder 1: [Group: "For the Biscuits"] 1 tbsp baking powder
SortOrder 2: [Group: "For the Biscuits"] 1/2 tsp salt
SortOrder 3: [Group: "For the Gravy"]    1 lb sausage
SortOrder 4: [Group: "For the Gravy"]    3 tbsp flour
SortOrder 5: [Group: null]               Salt and pepper to taste
```

Frontend groups by `GroupName` and preserves `SortOrder` within each group. Group display order is determined by the lowest `SortOrder` of any ingredient in that group.

## Implementation Files

| File | Purpose |
|------|---------|
| `Server/Domain/Recipes/RecipeIngredient.cs` | Entity with factory method, private setters |
| `Server/Domain/Recipes/IngredientUnit.cs` | SmartEnum value object with parse aliases |
| `Server/Databases/EntityConfigurations/RecipeIngredientConfiguration.cs` | EF Core entity configuration |
| `Server/Domain/Recipes/Dtos/RecipeIngredientDto.cs` | Read DTO |
| `Server/Domain/Recipes/Dtos/RecipeIngredientForCreationDto.cs` | Write DTO |
| `Server/Domain/Recipes/Dtos/ParseIngredientsDto.cs` | Parse request/response DTOs |
| `Server/Domain/Recipes/Features/UpdateRecipeIngredients.cs` | CQRS command — replace all |
| `Server/Domain/Recipes/Features/ParseIngredients.cs` | CQRS command — parse raw text |
| `Server/Domain/Recipes/Services/IngredientParser.cs` | Shared parsing logic |
| `Server/Domain/Recipes/Mappings/RecipeMapper.cs` | Add ingredient mapping methods |
| `Server/Domain/Recipes/Controllers/v1/RecipesController.cs` | Add ingredient endpoints |
| EF Migration | Add `RecipeIngredients` table |

### Changes to Existing Files

- **`Recipe.cs`** — Add `Ingredients` collection + `SetIngredients()` method
- **`RecipeDto.cs`** — Add `List<RecipeIngredientDto> Ingredients`
- **`RecipeSummaryDto.cs`** — Add `int IngredientCount` (lightweight, no full list)
- **`RecipeConfiguration.cs`** — Add navigation property config for Ingredients
- **`AppDbContext.cs`** — Add `DbSet<RecipeIngredient>`
- **`GetRecipe.cs`** — Eager load Ingredients
- **`GetRecipeList.cs`** — Project IngredientCount
- **`RecipeMapper.cs`** — Add ingredient mapping methods
