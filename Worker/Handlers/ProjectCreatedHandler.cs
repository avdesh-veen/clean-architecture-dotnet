using Microsoft.Extensions.Logging;
using SampleProject.Worker.Messages;
using SampleProject.Application.Services.Interfaces;
using SampleProject.Domain.Models;
using SampleProject.Worker.Temporal;

namespace SampleProject.Worker.Handlers;

/// <summary>
/// Wolverine message handler for ProjectCreatedMessage.
/// Handles post-creation workflows like notifications and provisioning.
/// </summary>
public class ProjectCreatedHandler
{
    private readonly ILogger<ProjectCreatedHandler> _logger;
    private readonly IBaseService<Project> _projectService;
    private readonly IProjectWorkflow _projectWorkflow;

    public ProjectCreatedHandler(
        ILogger<ProjectCreatedHandler> logger,
        IBaseService<Project> projectService,
        IProjectWorkflow projectWorkflow)
    {
        _logger = logger;
        _projectService = projectService;
        _projectWorkflow = projectWorkflow;
    }

    /// <summary>
    /// Handles the ProjectCreatedMessage by initiating workflows and notifications.
    /// This method signature follows Wolverine conventions.
    /// </summary>
    /// <param name="message">The project created message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task Handle(
        ProjectCreatedMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing ProjectCreatedMessage for project {ProjectId} in tenant {TenantId}",
            message.ProjectId,
            message.TenantId);

        try
        {
            // Verify project exists
            var project = await _projectService.GetByUuidAsync(message.ProjectId, cancellationToken);
            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} not found, skipping workflow", message.ProjectId);
                return;
            }

            // Start Temporal workflow for project provisioning
            await _projectWorkflow.RunAsync(message.ProjectId, message.TenantId);

            _logger.LogInformation(
                "Successfully initiated workflows for project {ProjectId}",
                message.ProjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process ProjectCreatedMessage for project {ProjectId}",
                message.ProjectId);
            throw; // Let Wolverine handle retry logic
        }
    }
}
