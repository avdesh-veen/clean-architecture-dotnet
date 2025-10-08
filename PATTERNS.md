# Patterns & Standards

This project follows a set of patterns and conventions to ensure consistency, security, and maintainability.

Core Conventions
- Entities inherit `BaseEntity` (Uuid, TenantId, IsDeleted).
- Auditable entities inherit `AuditableEntity` for Creation/Update timestamps and user ids.
- All database names use snake_case.
- Use `IInstaller` classes to register DI services; place them in `Installers/`.

CQRS + MediatR
- Implement features as `Request` -> `Handler` pairs in `Application/Requests` and `Application/Handlers`.
- Prefer small, focused handlers. Use FluentValidation for request validation.
- Pipeline behaviors: `ValidationBehavior`, `AuditEnrichmentBehavior`.

EF Core + Interceptors
- Register interceptors in `DbInstaller` and pass to `ApplicationDbContext`.
- Interceptors run in this order: `TenantEntityInterceptor`, `AuditableEntityInterceptor`, `SoftDeleteCascadeInterceptor`.
- Avoid manual assignment of `TenantId`, `CreatedBy`, `LastUpdatedBy`.

Soft Delete
- Use `IsDeleted = true` to delete entities; cascade handled by interceptor.
- Use `IgnoreQueryFilters()` only with clear justification and code comment.

Authorization
- Use `IAuthorizationService` for authorization checks.
- Objects implementing `IAuthorizationSubject` use subject format `"type:uuid"`.

Testing
- Integration tests use Testcontainers Postgres and `IntegrationTestWebApplicationFactory`.
- Generate test JWTs using `TestJwtTokenGenerator.GenerateTestToken(userId, tenantId)`.

Code Review Checklist
- Respect tenant isolation
- Verify interceptors are used correctly
- Return DTOs from controllers (do not return EF entities directly)
- Add/Update migrations with proper naming (`YYYYMMDDHHMMSS_Description`)
