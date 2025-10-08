# Architecture Overview

Project follows layered/clean architecture with inward dependencies:

- Presentation (Api, AppHost, Worker)
- Application (MediatR handlers, DTOs, services)
- Domain (Entities, ValueObjects, Enums)
- Infrastructure (EF Core DbContext, interceptors, persistence implementations)

Key responsibilities:
- Presentation: controllers and HTTP surface, mapping to/from DTOs, auth
- Application: business use-cases implemented as Request -> Handler pairs using MediatR
- Domain: pure business models and invariants (no external dependencies)
- Infrastructure: database, adapters, and service implementations

Important cross-cutting concerns:
- Multi-tenancy: `TenantId` on `BaseEntity`, global query filter in `ApplicationDbContext`
- Auditing: `AuditableEntity` fields applied via interceptors
- Soft-delete: `IsDeleted` flag and `SoftDeleteCascadeInterceptor`
- Authorization: OpenFGA integration; use `IAuthorizationService` for checks

Additions to this file should be reviewed with the architecture owner.
