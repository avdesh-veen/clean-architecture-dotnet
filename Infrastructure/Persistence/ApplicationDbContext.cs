using Microsoft.EntityFrameworkCore;
using SampleProject.Domain.Models;

namespace SampleProject.Infrastructure.Persistence;

/// <summary>
/// Minimal DbContext used in the sample. In the real project this includes interceptors,
/// global query filters for TenantId/IsDeleted, and naming conventions.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>Projects DbSet.</summary>
    public DbSet<Project> Projects { get; set; } = null!;

    /// <summary>
    /// Configure model. Add keys and any required mapping here.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // small example: configure Uuid as key
        modelBuilder.Entity<Project>().HasKey(p => p.Uuid);
    }
}
