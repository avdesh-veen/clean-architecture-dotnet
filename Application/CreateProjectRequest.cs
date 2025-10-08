using MediatR;

namespace SampleProject.Application.Requests;

/// <summary>
/// Request to create a new Project.
/// </summary>
/// <param name="Name">Project name.</param>
/// <param name="Description">Project description.</param>
public record CreateProjectRequest(string Name, string Description) : IRequest<ProjectResponse>;

namespace SampleProject.Application.Services.Interfaces;

/// <summary>
/// Generic base service interface for managing models.
/// Provides common methods for querying, creating, updating, and deleting models.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
public interface IBaseService<TModel>
    where TModel : class
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
