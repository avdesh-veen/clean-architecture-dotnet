using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SampleProject.Domain.Models;

namespace SampleProject.Infrastructure.Interceptors;

/// <summary>
/// Example interceptor that assigns a TenantId on entities when they are added.
/// </summary>
public class TenantEntityInterceptor : SaveChangesInterceptor
{
    private readonly Guid _tenantId;

    /// <summary>
    /// Initializes the interceptor with a tenant id.
    /// </summary>
    public TenantEntityInterceptor(Guid tenantId) => _tenantId = tenantId;

    /// <summary>
    /// Assign TenantId to newly added Project entities.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var entries = eventData.Context.ChangeTracker.Entries<Project>().Where(e => e.State == EntityState.Added);
        foreach (var e in entries) e.Entity.TenantId = _tenantId;
        return base.SavingChanges(eventData, result);
    }
}
