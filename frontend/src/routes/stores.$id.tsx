import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/stores/$id')({
  component: StoreDetailLayout,
})

function StoreDetailLayout() {
  return <Outlet />
}
