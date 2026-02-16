import { createFileRoute, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/collections/$id')({
  component: CollectionDetailLayout,
})

function CollectionDetailLayout() {
  return <Outlet />
}
