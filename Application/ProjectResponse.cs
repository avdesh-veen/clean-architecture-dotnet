namespace SampleProject.Application;

/// <summary>
/// Response DTO returned by project handlers.
/// </summary>
/// <param name="Uuid">Project UUID.</param>
/// <param name="Name">Project name.</param>
/// <param name="Description">Project description.</param>
/// <param name="CreationTime">Project creation time.</param>
public record ProjectResponse(Guid Uuid, string Name, string Description, DateTimeOffset CreationTime);

namespace SampleProject.Application.Services;

using SampleProject.Application.Services.Interfaces;
using SampleProject.Domain.Models;
using Serilog;

/// <summary>
/// Service layer for working with <see cref="Project"/> entities via the generic repository.
/// </summary>
/// <param name="repository">The project repository.</param>
public class ProjectService(IBaseRepository<Project> repository) : IBaseService<Project>
{
    private readonly IBaseRepository<Project> _repository = repository;
    private readonly ILogger _logger = Log.ForContext<ProjectService>();

    /// <inheritdoc />
    public IQueryable<Project> GetQueryable()
    {
        return _repository.GetQueryable();
    }

    /// <inheritdoc />
    public async Task<Project?> GetByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByUuidAsync(uuid, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Project> CreateAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        return await _repository.CreateAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Project?> UpdateAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        return await _repository.UpdateAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        Guid uuid,
        CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(uuid, cancellationToken);
    }
}
