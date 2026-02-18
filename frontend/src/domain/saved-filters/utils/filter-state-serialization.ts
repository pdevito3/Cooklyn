import type {
  Filter,
  FilterGroup,
  FilterState,
  DateValue,
} from '@/components/filter-builder/types'
import { isFilter } from '@/components/filter-builder/types'

function serializeDateValue(value: DateValue): DateValue & { __type: 'date' } {
  return {
    ...value,
    __type: 'date',
    startDate: value.startDate as unknown as Date,
    endDate: value.endDate as unknown as Date,
  }
}

function isDateValue(value: unknown): value is DateValue {
  return (
    typeof value === 'object' &&
    value !== null &&
    'mode' in value &&
    'startDate' in value
  )
}

function serializeFilterItem(
  item: Filter | FilterGroup,
): Filter | FilterGroup {
  if (isFilter(item)) {
    if (isDateValue(item.value)) {
      return {
        ...item,
        value: serializeDateValue(item.value),
      }
    }
    return item
  }

  // FilterGroup — recurse
  return {
    ...item,
    filters: item.filters.map(serializeFilterItem),
  }
}

export function serializeFilterState(state: FilterState): string {
  const serializable: FilterState = {
    ...state,
    filters: state.filters.map(serializeFilterItem),
  }
  return JSON.stringify(serializable)
}

function deserializeDateValue(value: Record<string, unknown>): DateValue {
  const result: DateValue = {
    mode: value.mode as DateValue['mode'],
    startDate: new Date(value.startDate as string),
  }
  if (value.endDate) {
    result.endDate = new Date(value.endDate as string)
  }
  if (value.exclude !== undefined) {
    result.exclude = value.exclude as boolean
  }
  if (value.dateType !== undefined) {
    result.dateType = value.dateType as DateValue['dateType']
  }
  return result
}

function deserializeFilterItem(
  item: Filter | FilterGroup,
): Filter | FilterGroup {
  if (isFilter(item)) {
    const value = item.value as unknown
    if (
      typeof value === 'object' &&
      value !== null &&
      ('__type' in (value as Record<string, unknown>) ||
        ('mode' in (value as Record<string, unknown>) &&
          'startDate' in (value as Record<string, unknown>)))
    ) {
      return {
        ...item,
        value: deserializeDateValue(value as Record<string, unknown>),
      }
    }
    return item
  }

  // FilterGroup — recurse
  return {
    ...item,
    filters: item.filters.map(deserializeFilterItem),
  }
}

export function deserializeFilterState(json: string): FilterState {
  const parsed: FilterState = JSON.parse(json)
  return {
    ...parsed,
    filters: parsed.filters.map(deserializeFilterItem),
  }
}
