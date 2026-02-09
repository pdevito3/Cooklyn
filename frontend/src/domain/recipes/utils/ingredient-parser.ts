import type { IngredientForCreationDto } from '../types'
import { INGREDIENT_UNITS } from '../types'

/**
 * Unit aliases for parsing — maps common abbreviations to canonical unit names.
 */
const UNIT_ALIASES: Record<string, string> = {
  // Cup
  c: 'Cup',
  'c.': 'Cup',
  cup: 'Cup',
  cups: 'Cup',
  // Tablespoon
  tbsp: 'Tablespoon',
  'tbsp.': 'Tablespoon',
  tbsps: 'Tablespoon',
  tbs: 'Tablespoon',
  'tbs.': 'Tablespoon',
  tablespoon: 'Tablespoon',
  tablespoons: 'Tablespoon',
  // Teaspoon
  tsp: 'Teaspoon',
  'tsp.': 'Teaspoon',
  tsps: 'Teaspoon',
  teaspoon: 'Teaspoon',
  teaspoons: 'Teaspoon',
  // Fluid Ounce
  'fl oz': 'FluidOunce',
  'fl. oz.': 'FluidOunce',
  floz: 'FluidOunce',
  'fluid ounce': 'FluidOunce',
  'fluid ounces': 'FluidOunce',
  // Milliliter
  ml: 'Milliliter',
  'ml.': 'Milliliter',
  mls: 'Milliliter',
  milliliter: 'Milliliter',
  milliliters: 'Milliliter',
  millilitre: 'Milliliter',
  millilitres: 'Milliliter',
  // Liter
  l: 'Liter',
  'l.': 'Liter',
  liter: 'Liter',
  liters: 'Liter',
  litre: 'Liter',
  litres: 'Liter',
  // Pint
  pt: 'Pint',
  'pt.': 'Pint',
  pint: 'Pint',
  pints: 'Pint',
  // Quart
  qt: 'Quart',
  'qt.': 'Quart',
  quart: 'Quart',
  quarts: 'Quart',
  // Gallon
  gal: 'Gallon',
  'gal.': 'Gallon',
  gallon: 'Gallon',
  gallons: 'Gallon',
  // Ounce
  oz: 'Ounce',
  'oz.': 'Ounce',
  ounce: 'Ounce',
  ounces: 'Ounce',
  // Pound
  lb: 'Pound',
  'lb.': 'Pound',
  lbs: 'Pound',
  'lbs.': 'Pound',
  pound: 'Pound',
  pounds: 'Pound',
  // Gram
  g: 'Gram',
  'g.': 'Gram',
  gr: 'Gram',
  gram: 'Gram',
  grams: 'Gram',
  // Kilogram
  kg: 'Kilogram',
  'kg.': 'Kilogram',
  kgs: 'Kilogram',
  kilogram: 'Kilogram',
  kilograms: 'Kilogram',
  // Piece
  pc: 'Piece',
  pcs: 'Piece',
  piece: 'Piece',
  pieces: 'Piece',
  // Whole
  whole: 'Whole',
  // Slice
  slice: 'Slice',
  slices: 'Slice',
  // Clove
  clove: 'Clove',
  cloves: 'Clove',
  // Pinch
  pinch: 'Pinch',
  pinches: 'Pinch',
  // Dash
  dash: 'Dash',
  dashes: 'Dash',
  // Can
  can: 'Can',
  cans: 'Can',
  // Bunch
  bunch: 'Bunch',
  bunches: 'Bunch',
  // Sprig
  sprig: 'Sprig',
  sprigs: 'Sprig',
  // Stick
  stick: 'Stick',
  sticks: 'Stick',
  // Head
  head: 'Head',
  heads: 'Head',
  // Bag
  bag: 'Bag',
  bags: 'Bag',
  // Jar
  jar: 'Jar',
  jars: 'Jar',
  // Package
  pkg: 'Package',
  'pkg.': 'Package',
  pkgs: 'Package',
  package: 'Package',
  packages: 'Package',
}

// Also add canonical names (lowercase) for matching
for (const unit of INGREDIENT_UNITS) {
  UNIT_ALIASES[unit.toLowerCase()] = unit
}

/**
 * Case-sensitive aliases where letter casing matters (T = Tablespoon, t = Teaspoon).
 */
const CASE_SENSITIVE_ALIASES: Record<string, string> = {
  T: 'Tablespoon',
  t: 'Teaspoon',
}

const AMOUNT_REGEX = /^(\d+\s+\d+\/\d+|\d+\/\d+|\d+\.?\d*)/

function parseAmount(text: string): number | null {
  const parts = text.split(/\s+/)

  // Mixed fraction: "1 1/2"
  if (parts.length === 2) {
    const whole = Number(parts[0])
    const frac = parseFraction(parts[1])
    if (!isNaN(whole) && frac !== null) {
      return whole + frac
    }
  }

  // Simple fraction: "1/2"
  const frac = parseFraction(text)
  if (frac !== null) return frac

  // Decimal or integer
  const num = Number(text)
  return isNaN(num) ? null : num
}

function parseFraction(text: string): number | null {
  const idx = text.indexOf('/')
  if (idx <= 0 || idx >= text.length - 1) return null

  const num = Number(text.slice(0, idx))
  const den = Number(text.slice(idx + 1))
  if (isNaN(num) || isNaN(den) || den === 0) return null

  return num / den
}

function tryMatchUnit(text: string): { unitName: string; consumed: number } | null {
  const words = text.split(/\s+/)

  // Try case-sensitive single-word match first (for T vs t)
  if (words.length >= 1) {
    const firstWord = words[0].replace(/[.,]$/, '')
    const caseSensitiveMatch = CASE_SENSITIVE_ALIASES[firstWord]
    if (caseSensitiveMatch) {
      return { unitName: caseSensitiveMatch, consumed: words[0].length }
    }
  }

  // Try two-word match (case-insensitive)
  if (words.length >= 2) {
    const twoWord = `${words[0]} ${words[1]}`.toLowerCase()
    const match = UNIT_ALIASES[twoWord]
    if (match) {
      return { unitName: match, consumed: words[0].length + 1 + words[1].length }
    }
  }

  // Try single-word match (case-insensitive)
  if (words.length >= 1) {
    const firstWord = words[0].replace(/[.,]$/, '').toLowerCase()
    const match = UNIT_ALIASES[firstWord]
    if (match) {
      return { unitName: match, consumed: words[0].length }
    }
  }

  return null
}

/**
 * Parse a single ingredient line into a structured DTO.
 */
export function parseLine(
  line: string,
  sortOrder: number
): IngredientForCreationDto {
  const trimmed = line.trim()
  let remaining = trimmed

  let amount: number | null = null
  let amountText: string | null = null
  let unit: string | null = null
  let name: string | null = null

  // Try to extract amount
  const amountMatch = remaining.match(AMOUNT_REGEX)
  if (amountMatch) {
    amountText = amountMatch[0].trim()
    amount = parseAmount(amountText)
    remaining = remaining.slice(amountMatch[0].length).trimStart()
  }

  // Try to match unit after amount
  if (remaining.length > 0 && amount !== null) {
    const unitMatch = tryMatchUnit(remaining)
    if (unitMatch) {
      unit = unitMatch.unitName
      remaining = remaining.slice(unitMatch.consumed).trimStart()
    }
  }

  // Remaining text is the ingredient name
  name = remaining.length > 0 ? remaining : null

  return {
    rawText: trimmed,
    name,
    amount,
    amountText,
    unit,
    customUnit: null,
    groupName: null,
    sortOrder,
  }
}

/**
 * Parse multi-line text into structured ingredient DTOs.
 */
export function parseText(text: string): IngredientForCreationDto[] {
  if (!text.trim()) return []

  const lines = text.split('\n')
  const results: IngredientForCreationDto[] = []
  let sortOrder = 0

  for (const line of lines) {
    if (!line.trim()) continue
    results.push(parseLine(line, sortOrder++))
  }

  return results
}
