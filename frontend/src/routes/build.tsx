import { createFileRoute } from '@tanstack/react-router'
import { useBuildInfo } from '@/domain/build-info/apis/get-build-info'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'

export const Route = createFileRoute('/build')({
  component: BuildPage,
})

function formatDate(dateStr: string): string {
  const date = new Date(dateStr)
  return date.toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function formatRelativeTime(dateStr: string): string {
  const date = new Date(dateStr)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffDays > 30) return formatDate(dateStr)
  if (diffDays > 0) return `${diffDays}d ago`
  if (diffHours > 0) return `${diffHours}h ago`
  if (diffMins > 0) return `${diffMins}m ago`
  return 'just now'
}

function BuildPage() {
  const { data: buildInfo, isLoading } = useBuildInfo()

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-40 w-full" />
        <Skeleton className="h-96 w-full" />
      </div>
    )
  }

  if (!buildInfo) return null

  const isDev = buildInfo.commitSha === 'development'
  const commitUrl = buildInfo.repositoryUrl
    ? `${buildInfo.repositoryUrl}/commit/`
    : null

  return (
    <div className="space-y-4">
      <Card>
        <CardHeader>
          <CardTitle>Build Details</CardTitle>
          <CardDescription>
            Information about the currently deployed build
          </CardDescription>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-[auto_1fr] gap-x-6 gap-y-3 text-sm">
            <dt className="text-muted-foreground font-medium">Status</dt>
            <dd>
              {isDev ? (
                <Badge variant="outline">Development</Badge>
              ) : (
                <Badge>Production</Badge>
              )}
            </dd>

            <dt className="text-muted-foreground font-medium">Commit</dt>
            <dd className="font-mono">
              {commitUrl && !isDev ? (
                <a
                  href={`${commitUrl}${buildInfo.commitSha}`}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-primary hover:underline"
                >
                  {buildInfo.shortSha}
                </a>
              ) : (
                buildInfo.shortSha
              )}
            </dd>

            <dt className="text-muted-foreground font-medium">Branch</dt>
            <dd className="font-mono">{buildInfo.branch}</dd>

            <dt className="text-muted-foreground font-medium">Built</dt>
            <dd>
              {isDev
                ? 'N/A'
                : formatDate(buildInfo.buildTimestamp)}
            </dd>
          </dl>
        </CardContent>
      </Card>

      {buildInfo.commits.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Recent Commits</CardTitle>
            <CardDescription>
              Last {buildInfo.commits.length} commits included in this build
            </CardDescription>
          </CardHeader>
          <CardContent className="p-0">
            <div className="divide-y">
              {buildInfo.commits.map((commit) => (
                <div
                  key={commit.sha}
                  className="flex items-start gap-3 px-6 py-3"
                >
                  <div className="min-w-0 flex-1">
                    <p className="truncate text-sm font-medium">
                      {commit.message}
                    </p>
                    <p className="text-muted-foreground text-xs">
                      {commit.author}
                      {' \u00B7 '}
                      {formatRelativeTime(commit.date)}
                    </p>
                  </div>
                  <div className="shrink-0">
                    {commitUrl ? (
                      <a
                        href={`${commitUrl}${commit.sha}`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-muted-foreground hover:text-primary font-mono text-xs transition-colors"
                      >
                        {commit.shortSha}
                      </a>
                    ) : (
                      <span className="text-muted-foreground font-mono text-xs">
                        {commit.shortSha}
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
