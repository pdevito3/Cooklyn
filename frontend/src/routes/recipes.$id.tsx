import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/recipes/$id')({
  component: RecipeIdLayout,
})

function RecipeIdLayout() {
  return <Outlet />
}
