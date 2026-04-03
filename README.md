# Cooklyn

A recipe management app for organizing recipes, planning meals, and building shopping lists. Single-user, self-hosted with no authentication.

Built with **.NET 10** and **React 19**, orchestrated by **.NET Aspire**.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Aspire AppHost                           │
│                 (Orchestration Layer)                       │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
    ┌─────────┐         ┌──────────┐         ┌─────────┐
    │ Server  │◄───────►│ Frontend │         │  MinIO  │
    │  (API)  │ (REST)  │  (React) │         │  (S3)   │
    └─────────┘         └──────────┘         └─────────┘
         │
    ┌──────────┐
    │PostgreSQL│
    └──────────┘
```

### Projects

- **Cooklyn.AppHost** — Aspire orchestration, defines all resources
- **Cooklyn.Server** — .NET 10 API (CQRS, MediatR, vertical slices)
- **frontend** — React 19 SPA (TanStack Router, React Query, Tailwind v4)

## Features

- **Recipes** — create, edit, import from URL, image uploads via S3
- **Collections** — organize recipes into groups
- **Meal Plans** — weekly meal planning with drag-and-drop
- **Shopping Lists** — auto-generated from meal plans or manual entry
- **Stores & Sections** — map items to store aisles for organized shopping
- **Saved Filters** — persist advanced filter/sort configurations

## Running the Application

```bash
cd Cooklyn.AppHost
aspire run
```

This starts the Aspire Dashboard, backend API, React dev server, PostgreSQL, and MinIO.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Aspire CLI](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dotnet-aspire-cli)
- [Node.js](https://nodejs.org/) with [pnpm](https://pnpm.io/)
- [Docker](https://www.docker.com/) (for PostgreSQL and MinIO containers)

## Tech Stack

### Backend
- .NET 10, CQRS with MediatR, vertical slice architecture
- PostgreSQL with EF Core, soft delete, audit fields
- MinIO (S3-compatible) for image storage
- Mapperly for compile-time DTO mapping
- QueryKit for filtering/sorting
- OpenTelemetry observability
- API versioning (`/api/v1/...`)

### Frontend
- React 19, TypeScript, Tailwind CSS v4
- TanStack Router (file-based), React Query, React Table
- Base UI + React Aria components
- React Hook Form + Zod validation
- Motion for animations, dnd-kit for drag-and-drop
- Keyboard shortcuts throughout

## License

MIT
