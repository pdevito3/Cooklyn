import { useState } from 'react'
import { createFileRoute } from '@tanstack/react-router'
import { useQueryClient } from '@tanstack/react-query'
import {
  RotateClockwiseIcon,
  CloudIcon,
} from '@hugeicons/core-free-icons'
import { HugeiconsIcon } from '@hugeicons/react'

import { useWeatherForecast } from '@/domain/weather/apis/get-weather'
import { WeatherKeys } from '@/domain/weather/apis/weather.keys'
import { Button } from '@/components/ui/button'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Skeleton } from '@/components/ui/skeleton'
import {
  Tooltip,
  TooltipTrigger,
  TooltipContent,
} from '@/components/ui/tooltip'

export const Route = createFileRoute('/')({
  component: Index,
})

function Index() {
  const [useCelsius, setUseCelsius] = useState(false)
  const queryClient = useQueryClient()

  const {
    data: weatherData = [],
    isLoading: loading,
    error,
    isFetching,
  } = useWeatherForecast()

  const handleRefresh = () => {
    queryClient.invalidateQueries({ queryKey: WeatherKeys.all })
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString(undefined, {
      weekday: 'short',
      month: 'short',
      day: 'numeric',
    })
  }

  return (
    <div className="min-h-screen bg-background">
      <main className="max-w-4xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">
              Weather Forecast
            </h1>
            <p className="text-muted-foreground mt-1">
              .NET Aspire + React
            </p>
          </div>
        </div>

        {/* Controls Card */}
        <Card className="mb-6">
          <CardContent className="pt-6">
            <div className="flex flex-wrap items-center justify-between gap-4">
              {/* Left: Unit toggle */}
              <div className="flex items-center gap-4">
                <div className="flex items-center rounded-lg border border-border p-1 gap-1">
                  <Button
                    variant={!useCelsius ? 'default' : 'ghost'}
                    size="sm"
                    onClick={() => setUseCelsius(false)}
                  >
                    °F
                  </Button>
                  <Button
                    variant={useCelsius ? 'default' : 'ghost'}
                    size="sm"
                    onClick={() => setUseCelsius(true)}
                  >
                    °C
                  </Button>
                </div>
              </div>

              {/* Right: Refresh button */}
              <Tooltip>
                <TooltipTrigger
                  render={
                    <Button
                      variant="outline"
                      onClick={handleRefresh}
                      disabled={isFetching}
                    >
                      <HugeiconsIcon
                        icon={RotateClockwiseIcon}
                        className={`w-4 h-4 mr-2 ${isFetching ? 'animate-spin' : ''}`}
                      />
                      Refresh
                    </Button>
                  }
                />
                <TooltipContent>Fetch latest weather data</TooltipContent>
              </Tooltip>
            </div>
          </CardContent>
        </Card>

        {/* Error Message */}
        {error && (
          <div className="bg-destructive/10 border border-destructive/20 text-destructive rounded-lg p-4 mb-6">
            <p className="font-medium">Error loading weather data</p>
            <p className="text-sm mt-1">
              {error instanceof Error
                ? error.message
                : 'Failed to fetch weather data'}
            </p>
          </div>
        )}

        {/* Loading State */}
        {loading && weatherData.length === 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {[...Array(6)].map((_, i) => (
              <Card key={i}>
                <CardContent className="pt-6">
                  <Skeleton className="h-4 w-24 mb-2" />
                  <Skeleton className="h-6 w-32 mb-4" />
                  <Skeleton className="h-10 w-20" />
                </CardContent>
              </Card>
            ))}
          </div>
        )}

        {/* Weather Grid */}
        {weatherData.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {weatherData.map((forecast, index) => (
              <WeatherCard
                key={index}
                forecast={forecast}
                useCelsius={useCelsius}
                formatDate={formatDate}
              />
            ))}
          </div>
        )}

        {/* Empty State */}
        {!loading && !error && weatherData.length === 0 && (
          <div className="text-center py-12">
            <HugeiconsIcon
              icon={CloudIcon}
              className="w-12 h-12 mx-auto text-muted-foreground mb-4"
            />
            <p className="text-muted-foreground">No weather data available</p>
            <Button variant="outline" className="mt-4" onClick={handleRefresh}>
              Try Again
            </Button>
          </div>
        )}
      </main>
    </div>
  )
}

interface WeatherCardProps {
  forecast: {
    date: string
    temperatureC: number
    temperatureF: number
    summary: string
  }
  useCelsius: boolean
  formatDate: (date: string) => string
}

function WeatherCard({
  forecast,
  useCelsius,
  formatDate,
}: WeatherCardProps) {
  return (
    <Card className="transition-shadow hover:shadow-md">
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between">
          <div>
            <p className="text-sm font-medium text-muted-foreground uppercase tracking-wide">
              {formatDate(forecast.date)}
            </p>
            <CardTitle className="mt-1">{forecast.summary}</CardTitle>
          </div>
          <HugeiconsIcon
            icon={CloudIcon}
            className="w-6 h-6 text-muted-foreground"
          />
        </div>
      </CardHeader>
      <CardContent>
        <div className="pt-3 border-t">
          <p className="text-3xl font-bold">
            {useCelsius ? forecast.temperatureC : forecast.temperatureF}°
          </p>
          <p className="text-sm text-muted-foreground">
            {useCelsius ? 'Celsius' : 'Fahrenheit'}
          </p>
        </div>
      </CardContent>
    </Card>
  )
}
