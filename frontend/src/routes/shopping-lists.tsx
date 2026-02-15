import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/shopping-lists')({
  component: ShoppingListsLayout,
})

function ShoppingListsLayout() {
  return <Outlet />
}
