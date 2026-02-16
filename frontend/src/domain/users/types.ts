export interface UserDto {
  id: string
  tenantId: string
  firstName: string
  lastName: string
  fullName: string
  identifier: string
  email: string
  username: string
  role: string
  defaultStoreId: string | null
  permissions: string[]
}

export interface UpdateUserDefaultStoreDto {
  storeId: string | null
}
