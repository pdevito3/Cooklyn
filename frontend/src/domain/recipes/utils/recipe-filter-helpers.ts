import type { FilterOption } from '@/components/filter-builder/types'
import { RECIPE_RATINGS } from '@/domain/recipes/types'

/**
 * Properties that need ^^ (IN) operator expansion to individual == conditions.
 * - Navigation collections (tags, flags): QueryKit's ^^ doesn't work with
 *   .Select().HasQueryName(); == generates .Any() LINQ expressions instead.
 * - Value objects (rating): QueryKit can't parse array values for value object
 *   types, but == with individual strings works fine.
 */
const EXPAND_IN_PROPERTIES = new Set(['tags', 'flags', 'rating'])

/**
 * Flag filter options mapping SmartEnum names (DB values) to display labels.
 */
export const FLAG_FILTER_OPTIONS: FilterOption[] = [
  { value: 'Vegan', label: 'Vegan' },
  { value: 'Vegetarian', label: 'Vegetarian' },
  { value: 'GlutenFree', label: 'Gluten-Free' },
  { value: 'DairyFree', label: 'Dairy-Free' },
  { value: 'NutFree', label: 'Nut-Free' },
  { value: 'HighProtein', label: 'High-Protein' },
  { value: 'HighFiber', label: 'High-Fiber' },
  { value: 'LowCarb', label: 'Low-Carb' },
  { value: 'LowFat', label: 'Low-Fat' },
  { value: 'LowSodium', label: 'Low-Sodium' },
  { value: 'Keto', label: 'Keto' },
  { value: 'Paleo', label: 'Paleo' },
  { value: 'Whole30', label: 'Whole30' },
  { value: 'QuickAndEasy', label: 'Quick & Easy' },
  { value: 'MealPrep', label: 'Meal Prep' },
  { value: 'FreezerFriendly', label: 'Freezer-Friendly' },
  { value: 'OnePot', label: 'One-Pot' },
  { value: 'KidFriendly', label: 'Kid-Friendly' },
]

/**
 * Rating filter options (excluding "Not Rated").
 */
export const RATING_FILTER_OPTIONS: FilterOption[] = RECIPE_RATINGS.filter(
  (r) => r !== 'Not Rated',
).map((r) => ({ value: r, label: r }))

/**
 * Parse comma-separated array values from a QueryKit string, respecting quotes.
 * e.g. 'Vegan, "Loved It", GlutenFree' → ['Vegan', '"Loved It"', 'GlutenFree']
 */
function parseArrayValues(valuesStr: string): string[] {
  const values: string[] = []
  let current = ''
  let inQuotes = false

  for (const char of valuesStr) {
    if (char === '"') {
      inQuotes = !inQuotes
      current += char
    } else if (char === ',' && !inQuotes) {
      const trimmed = current.trim()
      if (trimmed) values.push(trimmed)
      current = ''
    } else {
      current += char
    }
  }

  const trimmed = current.trim()
  if (trimmed) values.push(trimmed)

  return values
}

/**
 * Post-processes a QueryKit filter string to expand ^^ (IN) operators on
 * properties where QueryKit can't handle the array syntax natively (navigation
 * collections and value objects). Converts to individual == conditions.
 *
 * Examples:
 *   flags ^^ [Vegan, GlutenFree]          → (flags == Vegan || flags == GlutenFree)
 *   flags %^^ [Vegan, GlutenFree]         → (flags == Vegan && flags == GlutenFree)
 *   flags !^^ [Vegan, GlutenFree]         → (flags != Vegan && flags != GlutenFree)
 *   tags ^^ ["My Tag", Dinner]            → (tags == "My Tag" || tags == Dinner)
 *   rating ^^ ["Loved It", "Liked It"]    → (rating == "Loved It" || rating == "Liked It")
 */
export function transformCollectionFilters(queryKitString: string): string {
  return queryKitString.replace(
    /(\w+)\s+(%?!?\^\^\*?)\s+\[([^\]]*)\]/g,
    (match, property: string, operator: string, valuesStr: string) => {
      if (!EXPAND_IN_PROPERTIES.has(property)) {
        return match
      }

      const values = parseArrayValues(valuesStr)
      if (values.length === 0) return match

      const isMatchAll = operator.startsWith('%')
      const isNegated = operator.includes('!')
      const isCaseInsensitive = operator.endsWith('*')

      const equalOp = isNegated ? '!=' : '=='
      const caseSuffix = isCaseInsensitive ? '*' : ''
      const joinOp = isMatchAll || isNegated ? ' && ' : ' || '

      const conditions = values.map(
        (v) => `${property} ${equalOp}${caseSuffix} ${v}`,
      )

      return `(${conditions.join(joinOp)})`
    },
  )
}
