import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/shopping-lists/$id')({
  component: ShoppingListDetailLayout,
})

function ShoppingListDetailLayout() {
  return <Outlet />
}
