# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

.NET Aspire application with React frontend for recipe management. Single-user, private-server deployment with no authentication. Uses CQRS architecture with vertical slice patterns.

## Running the Application

```bash
# Start everything (recommended)
cd Cooklyn.AppHost
aspire run

# Or use dotnet run
dotnet run --project Cooklyn.AppHost
```

This starts: Aspire Dashboard, Backend API, React dev server, PostgreSQL, and MinIO.

## Build Commands

```bash
# Backend
dotnet build

# Frontend
cd frontend
pnpm install
pnpm dev      # dev server
pnpm build    # production build
pnpm lint     # ESLint
```

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
    ┌─────────┐
    │PostgreSQL│
    └─────────┘
```

### Projects

- **Cooklyn.AppHost** - Aspire orchestration, defines all resources
- **Cooklyn.Server** - Backend API (.NET 10, CQRS, serves SPA in production)
- **frontend** - React SPA (React 19, TanStack Router, React Query, Tailwind v4)

## Backend Patterns

### Domain Structure

Each domain entity follows this folder structure:

```
Server/Domain/
└── [EntityName]s/
    ├── [EntityName].cs              # Domain entity
    ├── Controllers/v1/              # API controllers
    ├── Dtos/                        # DTOs (read/write)
    ├── Features/                    # CQRS commands/queries
    ├── Mappings/                    # Mapperly mappers
    ├── Models/                      # Internal models
    └── DomainEvents/                # Domain events
```

### Key Backend Concepts

- **Rich Domain Entities**: Private setters, factory methods, encapsulated business logic
- **CQRS with MediatR**: Commands for writes, Queries for reads
- **Vertical Slice Architecture**: Each feature is self-contained
- **Mapperly**: Compile-time source generation for DTO mapping
- **QueryKit**: Filtering and sorting for list endpoints
- **Domain Events**: Queued in entities, dispatched on SaveChanges

### Service Registration

Automatic DI registration using marker interfaces:

```csharp
// Default: Scoped lifetime (no marker needed)
public interface IMyService { }
public class MyService : IMyService { }

// Singleton lifetime
public interface IMySingletonService { }
public class MySingletonService : IMySingletonService, ISingletonService { }

// Transient lifetime
public interface IMyTransientService { }
public class MyTransientService : IMyTransientService, ITransientService { }
```

Services are auto-registered by calling `builder.Services.AddApplicationServices()` in Program.cs.

## API Versioning

Location: `Server/Resources/Extensions/ApiVersioningExtension.cs`

URL segment versioning at `/api/v{version}/...`:

```csharp
// Controller-based (recommended)
[Route("api/v{v:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public sealed class CustomersController : ControllerBase { }
```

## Telemetry

Location: `Server/Resources/Telemetry.cs`

Centralized OpenTelemetry instrumentation with pre-configured metrics. View traces and metrics in the Aspire Dashboard.

## Frontend Patterns

### Project Structure

```
frontend/src/
├── domain/                 # Domain-organized API hooks and types
├── components/
│   ├── ui/                # 40+ reusable UI components
│   ├── filter-builder/    # Advanced filter component
│   ├── app-sidebar.tsx    # Main navigation sidebar
│   ├── theme-provider.tsx # Theme context
│   └── theme-toggle.tsx   # Light/dark/system toggle
├── routes/                 # TanStack Router file-based routes
├── hooks/                  # Custom React hooks
├── lib/                    # Utilities (cn, api-client, etc.)
└── main.tsx               # App entry point
```

### API Client

Location: `frontend/src/lib/api-client.ts`

Axios instance with Problem Details error handling. In development, Vite proxies `/api` to the server.

### Routing (TanStack Router)

File-based routing in `frontend/src/routes/`.

### Settings

App-level settings are stored via `GET/PUT /api/v1/settings/{key}`. Frontend uses `useDefaultStore()` from `@/domain/settings/apis/get-setting` for the default store preference.

## Database Patterns

### AppDbContext Features

Location: `Server/Databases/AppDbContext.cs`

- **Soft Delete**: Entities with `IsDeleted` are filtered automatically
- **Audit Fields**: `CreatedOn`, `LastModifiedOn` auto-populated
- **Domain Events**: Dispatched after SaveChanges via MediatR

### Migrations

```bash
# Add a migration
dotnet ef migrations add MyMigration --project Cooklyn.Server

# Apply migrations (auto-runs on startup in development)
dotnet ef database update --project Cooklyn.Server
```

## Working with Aspire

1. Always run `aspire run` before making changes to verify starting state
2. Changes to `AppHost.cs` require restart; other code changes hot-reload
3. Use Aspire MCP tools to check resource status and debug issues
4. The Aspire workload is obsolete - never attempt to install it

## Key Configuration

- **Centralized Package Management**: All NuGet versions in `Directory.Packages.props`
- **Frontend Package Manager**: pnpm (not npm)
- **Docker**: Use `docker compose` with a space (not `docker-compose`)

## MCP Tools Available

- **Aspire MCP**: Resource management, logs, traces, integrations
- **Playwright MCP**: Browser automation for functional testing

## Adding a New Feature

1. **Create Domain Entity** - Rich entity with private setters, factory methods
2. **Add Entity Configuration** - Configure in `Databases/EntityConfigurations/`, run migration
3. **Create DTOs and Mappings** - Read DTOs, Creation DTOs, Update DTOs with Mapperly mapper
4. **Implement Features** - Add/Get/Update/Delete commands and queries with MediatR
5. **Add Controller** - Thin controller delegating to MediatR with versioning

## React
- Base UI's SelectValue renders the raw value from SelectItem. Since we always pass entity IDs as value, a self-closing <SelectValue /> shows the ID instead of the label. Always provide a children render function that maps
  the ID back to a display name.      
- Use hotkeys for common actions (e.g. Ctrl+K to open command palette, `C` for Create, `E` for edit, `Delete` for delete, etc.) or sidenav to speed up development and testing, including a kbd hint. in buttons and UI elements.
- Forms should be able to be submitted with enter like a normal form when appropriate, but should always use Cmd+Enter
- No barrel files

## Notes

- Use conventional commit naming for git commits with the area, for example `feat(epub): Add enhanced text reading`
- Ensure we have high signal test coverage to confirm functionality of features and catch regressions
