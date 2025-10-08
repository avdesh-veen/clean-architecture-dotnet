# End-to-End Flow: Request → Handler → Persistence → Response

This document explains how a typical request moves through the Vita Admin Service codebase, with concrete folder and file examples so clients can map concepts to physical files.

Example scenario: Create a new Project

1) Client → API Controller
- Folder: `Vita.AdminService.Api/Controllers/`
- File: `ProjectsController.cs`

What happens:
- Controller receives `POST /api/projects` with JSON body.
- Controller maps or forwards the request as a `CreateProjectRequest` (a MediatR request) and calls `_mediator.Send(request)`.

2) Application Layer → MediatR Request and Handler
- Folder: `Vita.AdminService.Application/Requests/Projects/`
- File: `CreateProjectRequest.cs`
- Folder: `Vita.AdminService.Application/Handlers/Projects/`
- File: `CreateProjectHandler.cs`

What happens:
- `CreateProjectRequest` is a simple DTO/record with the required fields.
- `CreateProjectHandler` implements `IRequestHandler<CreateProjectRequest, ProjectResponse>`.
- The handler constructs a domain `Project` entity and adds it to `ApplicationDbContext.Projects`.

3) Infrastructure Layer → ApplicationDbContext + Interceptors
- Folder: `Vita.AdminService.Infrastructure/Persistence/`
- File: `ApplicationDbContext.cs` (DbContext definition)
- Folder: `Vita.AdminService.Infrastructure/Persistence/Interceptors/`
- Files: `TenantEntityInterceptor.cs`, `AuditableEntityInterceptor.cs`, `SoftDeleteCascadeInterceptor.cs`

What happens:
- `ApplicationDbContext` has DbSets like `DbSet<Project> Projects` and config in `OnModelCreating` (snake_case naming, global query filter for TenantId + IsDeleted).
- Interceptors are added to the DbContext and run during `SaveChangesAsync()`:
  - `TenantEntityInterceptor` sets `TenantId` for new entities.
  - `AuditableEntityInterceptor` sets `CreationTime`, `CreatedBy`, etc.
  - `SoftDeleteCascadeInterceptor` ensures soft-delete cascades.

4) Persistence → Database
- Migrations live in `Vita.AdminService.Infrastructure/Migrations/`
- On `SaveChangesAsync()` EF Core translates inserts/updates into SQL and writes to PostgreSQL.

5) Response Mapping → Controller Response
- Folder: `Vita.AdminService.Application/Responses/Projects/`
- File: `ProjectResponse.cs`
- Mapping usually done via `IMapper` (AutoMapper) or manual projection in the handler.
- The handler returns `ProjectResponse` to the controller; controller returns `201 Created` with the response body.

File & Folder Map (concise):

- Vita.AdminService.Api/
  - Controllers/
    - `ProjectsController.cs`

- Vita.AdminService.Application/
  - Requests/Projects/
    - `CreateProjectRequest.cs`
  - Handlers/Projects/
    - `CreateProjectHandler.cs`
  - Responses/Projects/
    - `ProjectResponse.cs`
  - Validators/Projects/
    - `CreateProjectRequestValidator.cs`

- Vita.AdminService.Domain/
  - Models/
    - `Project.cs` (inherits `BaseEntity`)

- Vita.AdminService.Infrastructure/
  - Persistence/
    - `ApplicationDbContext.cs`
    - Interceptors/
      - `TenantEntityInterceptor.cs`
      - `AuditableEntityInterceptor.cs`
      - `SoftDeleteCascadeInterceptor.cs`
    - Migrations/

Notes & Best Practices
- Controllers: accept DTOs, forward to MediatR, return DTOs (do not return EF entities directly).
- Handlers: small, focused; call `SaveChangesAsync()` once per unit-of-work.
- Interceptors: own tenant/audit/soft-delete responsibilities; do not be overridden by handlers.
- Tests: Integration tests exercise the full flow via HTTP client and check DB state with `IgnoreQueryFilters()` only when validating soft-delete/audit fields.
