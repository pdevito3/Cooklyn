import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/meal-plan')({
  component: MealPlanLayout,
})

function MealPlanLayout() {
  return <Outlet />
}
