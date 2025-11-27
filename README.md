# Admin Service - Architecture & Code Standards

This comprehensive documentation showcases the sophisticated architecture, patterns, and standards used in the **Admin Service** - a multi-tenant ASP.NET Core 9.0 admin service with PostgreSQL, implementing clean architecture with CQRS, advanced multi-tenancy, and enterprise-grade authorization.

## 🏗️ Architecture Overview

### **Technology Stack**
- **.NET 9.0** with ASP.NET Core
- **PostgreSQL 17** with EF Core 9.0
- **Keycloak** for JWT authentication
- **OpenFGA** for relationship-based authorization
- **Wolverine** for message bus and background processing
- **Temporal** for long-running workflow orchestration
- **.NET Aspire** for local development orchestration
- **MediatR** for CQRS pattern implementation

### **Solution Structure (Clean/Layered Architecture)**
```
AdminService.Api          # REST + OData controllers, JWT auth, middleware
AdminService.Worker       # Background processing, Wolverine messaging, Temporal workflows
AdminService.Application  # MediatR handlers, validation, mapping, business services
AdminService.Domain       # Entities, value objects, enums, domain events
AdminService.Infrastructure # EF Core context, interceptors, repositories, external adapters
AdminService.Shared       # Cross-cutting interfaces, constants, extensions
AdminService.ServiceDefaults # OpenTelemetry, health checks, service discovery
AdminService.AppHost      # .NET Aspire orchestration for local development
AdminService.Tests        # Integration tests with Testcontainers
```

### **Dependency Flow (Inward Only)**
```
Api/Worker → Application → Domain
         ↓              ↓
    Infrastructure ←────┘
```

## 🔑 Core Enterprise Patterns

### **Multi-Tenancy & Data Isolation**
- **Global Query Filter**: Automatically applies `TenantId == currentTenant AND IsDeleted == false`
- **Tenant Interceptor**: Auto-assigns `TenantId` from JWT claims on entity creation
- **Tenant Context**: Extracted from JWT `tenantId` claim via middleware
- **Cross-tenant queries**: Require explicit `IgnoreQueryFilters()` with justification

### **Advanced Entity Framework Interceptors**
1. **TenantEntityInterceptor**: Assigns `TenantId` on Added entities
2. **AuditableEntityInterceptor**: Manages creation/update timestamps and user tracking
3. **SoftDeleteCascadeInterceptor**: Cascades soft-delete through navigation properties

### **CQRS with MediatR**
- **Request/Handler Pattern**: Every feature implemented as `Request → Handler`
- **Pipeline Behaviors**: Validation (FluentValidation), audit enrichment (Serilog)
- **Generic Requests**: `GetRequest<T>`, `DeleteRequest<T>`, `BulkDeleteRequest<T>`
- **Dual Controllers**: REST for CRUD + OData for advanced querying

### **Enterprise Authentication & Authorization**
- **Keycloak JWT**: Claims-based authentication with tenant isolation
- **OpenFGA Integration**: Relationship-based access control (ReBAC)
- **Identity Normalization**: External provider mapping via `identity_providers` + `user_identities`
- **Authorization Subjects**: Domain objects implement `IAuthorizationSubject`

## 📁 Documentation Contents

### **Core Documentation**
- **`ARCHITECTURE.md`** - Detailed layer responsibilities and cross-cutting concerns
- **`PATTERNS.md`** - CQRS, interceptors, soft delete, multi-tenancy conventions
- **`FLOW.md`** - End-to-end request flow with concrete file examples
- **`sample-project/`** - Production-quality sample demonstrating all patterns

### **Sample Project Highlights**
The `sample-project/` folder contains a comprehensive example showing:

#### **Domain Layer** (`Domain/`)
```csharp
// BaseEntity with multi-tenancy and soft delete
public abstract class BaseEntity
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
}

// Project entity with proper inheritance
public class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
}
```

#### **Application Layer** (`Application/`)
- **Services**: `IBaseService<T>` with tenant-aware queryables
- **Handlers**: MediatR request handlers with validation pipeline
- **DTOs**: Request/Response objects with FluentValidation

#### **API Layer** (`Api/`)
- **REST Controllers**: Standard CRUD with MediatR dispatch
- **OData Controllers**: Advanced querying with `[EnableQuery]`
- **Authentication**: Keycloak JWT token validation
- **Authorization**: OpenFGA relationship checks

#### **Infrastructure Layer** (`Infrastructure/`)
- **DbContext**: EF Core with interceptors and global query filters
- **Interceptors**: Tenant isolation, auditing, soft delete cascading
- **Services**: Keycloak integration, OpenFGA client implementations

#### **Worker Layer** (`Worker/`)
- **Wolverine Handlers**: Async message processing patterns
- **Temporal Workflows**: Long-running business processes with activities

## 🚀 Enterprise Features Demonstrated

### **Keycloak Integration**
```csharp
// JWT claims extraction with fallback priority
var userId = GetClaimValue("vitaUserId") ??
             GetClaimValue(ClaimTypes.NameIdentifier) ??
             GetClaimValue("sub") ??
             GetClaimValue("id");
```

### **OpenFGA Authorization**
```csharp
// Relationship-based access control
var canEdit = await authService.CheckAsync(
    user: $"user:{userId}",
    relation: "editor",
    @object: $"project:{projectId}"
);
```

### **Wolverine Messaging**
```csharp
// Async message handling with retry policies
public static async Task Handle(
    ProjectCreatedMessage message,
    IProjectService projectService,
    ILogger<ProjectCreatedHandler> logger)
```

### **Temporal Workflows**
```csharp
// Long-running workflow orchestration
[Workflow]
public class ProjectWorkflow
{
    [WorkflowRun]
    public async Task<ProjectResult> RunAsync(ProjectRequest request)
    {
        // Provision resources, send notifications, setup permissions
        await Workflow.ExecuteActivityAsync<IProvisioningActivity>(...)
    }
}
```

## 🎯 Production-Ready Patterns

### **Entity Design Standards**
- All entities inherit from `BaseEntity` (UUID, TenantId, IsDeleted)
- Audit fields via `AuditableEntity` (managed by interceptors)
- Database naming: snake_case applied universally
- Timestamps: `timestamp with time zone` for PostgreSQL

### **Request/Response Flow**
```
HTTP Request → Controller → MediatR Request → Handler → Service → Repository → Database
                    ↑                                        ↓
              Authorization ←←← OpenFGA ←←← Domain Object
```

### **Testing Strategy**
- **Integration Tests**: Testcontainers with real PostgreSQL
- **Tenant Isolation**: Verified in every test with separate GUIDs
- **JWT Generation**: `TestJwtTokenGenerator` for authentication context
- **Data Cleanup**: Automatic via Testcontainers disposal

## 🔧 Development Workflow

### **Adding New Entities (8-Step Process)**
1. Create domain entity (inherit from `BaseEntity`)
2. Add EF Core configuration in `ApplicationDbContext.OnModelCreating`
3. Create migration with `dotnet ef migrations add`
4. Create Request/Response DTOs in Application layer
5. Create MediatR handler with validation
6. Create REST controller with authorization
7. Create OData controller for complex querying
8. Add integration tests with tenant isolation verification

### **Local Development with Aspire**
- Run `Vita.AdminService.AppHost` for full infrastructure
- Automatic service discovery and container orchestration
- Includes PostgreSQL, Keycloak, OpenFGA, Temporal, pgAdmin

## 📚 Usage Instructions

### **For Prospects/Clients**
1. **Review Architecture**: Start with `ARCHITECTURE.md` for high-level overview
2. **Explore Patterns**: Check `PATTERNS.md` for implementation details
3. **Follow Flow**: See `FLOW.md` for request lifecycle
4. **Study Examples**: Examine `sample-project/` for concrete implementations

### **For Development Teams**
1. **Clone Repository**: All patterns ready for adaptation
2. **Follow Conventions**: Entity inheritance, interceptor usage, MediatR patterns
3. **Extend Safely**: Use provided base classes and interfaces
4. **Test Thoroughly**: Integration tests with tenant isolation verification

---

This documentation demonstrates enterprise-grade .NET development with advanced multi-tenancy, sophisticated authorization, modern messaging patterns, and production-ready architecture. The sample project provides concrete examples of all patterns and can serve as a foundation for similar enterprise applications.
