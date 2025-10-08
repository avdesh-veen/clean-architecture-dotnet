namespace SampleProject.Domain.Models;

/// <summary>
/// Base entity containing common identity and tenant fields.
/// </summary>
public class BaseEntity
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Auditable entity with creation/update metadata.
/// </summary>
public class AuditableEntity : BaseEntity
{
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public Guid? LastUpdatedBy { get; set; }
    public DateTimeOffset? LastUpdatedTime { get; set; }
}

/// <summary>
/// Domain entity representing a Project (inherits auditing and base fields).
/// </summary>
public class Project : AuditableEntity
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Generic base repository interface for data access operations.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
public interface IBaseRepository<TModel> where TModel : class
{
    /// <summary>
    /// Retrieves an <see cref="IQueryable{T}"/> collection of all models.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TModel}"/> representing all models.</returns>
    IQueryable<TModel> GetQueryable();

    /// <summary>
    /// Retrieves a specific model by its unique identifier (UUID).
    /// </summary>
    /// <param name="uuid">The unique identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TModel?> GetByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new model.
    /// </summary>
    /// <param name="model">The model data to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The created model.</returns>
    Task<TModel> CreateAsync(
        TModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing model.
    /// </summary>
    /// <param name="model">The model data to update.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The updated model if found; otherwise, null.</returns>
    Task<TModel?> UpdateAsync(
        TModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a model by its unique identifier (UUID).
    /// </summary>
    /// <param name="uuid">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>True if the entity was found and deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(
        Guid uuid,
        CancellationToken cancellationToken = default);
}
