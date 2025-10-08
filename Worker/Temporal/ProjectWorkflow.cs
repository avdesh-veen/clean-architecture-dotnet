using Temporalio.Activities;
using Microsoft.Extensions.Logging;
using SampleProject.Authorization;
using SampleProject.Worker.Temporal;

namespace SampleProject.Worker.Temporal;

/// <summary>
/// Implementation of Temporal workflow for project lifecycle management.
/// Demonstrates long-running workflows with activities, signals, and queries.
/// </summary>
public class ProjectWorkflow : IProjectWorkflow
{
    private string _status = "initializing";
    private readonly ILogger<ProjectWorkflow> _logger;

    public ProjectWorkflow(ILogger<ProjectWorkflow> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task RunAsync(Guid projectId, Guid tenantId)
    {
        _logger.LogInformation("Starting project workflow for {ProjectId} in tenant {TenantId}", projectId, tenantId);

        try
        {
            _status = "provisioning";

            // Step 1: Provision resources
            var provisioningResult = await Workflow.ExecuteActivityAsync(
                (IProjectActivities activities) => activities.ProvisionResourcesAsync(projectId, tenantId, default),
                new ActivityOptions
                {
                    ScheduleToCloseTimeout = TimeSpan.FromMinutes(10),
                    RetryPolicy = new RetryPolicy
                    {
                        InitialInterval = TimeSpan.FromSeconds(1),
                        MaximumInterval = TimeSpan.FromMinutes(1),
                        BackoffCoefficient = 2.0,
                        MaximumAttempts = 3
                    }
                });

            if (!provisioningResult.Success)
            {
                _logger.LogError("Provisioning failed for project {ProjectId}: {Error}", projectId, provisioningResult.ErrorMessage);
                _status = "failed";
                return;
            }

            _status = "configuring_permissions";

            // Step 2: Configure permissions
            await Workflow.ExecuteActivityAsync(
                (IProjectActivities activities) => activities.ConfigurePermissionsAsync(projectId, tenantId, default),
                new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5) });

            _status = "sending_notifications";

            // Step 3: Send notifications
            await Workflow.ExecuteActivityAsync(
                (IProjectActivities activities) => activities.SendNotificationsAsync(projectId, tenantId, "project_created", default),
                new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(2) });

            _status = "completed";
            _logger.LogInformation("Project workflow completed successfully for {ProjectId}", projectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Project workflow failed for {ProjectId}", projectId);
            _status = "failed";
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateConfigurationAsync(string configuration)
    {
        _logger.LogInformation("Received configuration update: {Configuration}", configuration);
        _status = "updating_configuration";

        // Handle configuration update
        await Task.Delay(TimeSpan.FromSeconds(1)); // Simulate work

        _status = "configuration_updated";
    }

    /// <inheritdoc />
    public string GetStatus()
    {
        return _status;
    }
}

/// <summary>
/// Implementation of Temporal activities for project operations.
/// Activities interact with external systems and are automatically retried on failure.
/// </summary>
public class ProjectActivities : IProjectActivities
{
    private readonly ILogger<ProjectActivities> _logger;
    private readonly IOpenFgaService _openFgaService;

    public ProjectActivities(
        ILogger<ProjectActivities> logger,
        IOpenFgaService openFgaService)
    {
        _logger = logger;
        _openFgaService = openFgaService;
    }

    /// <inheritdoc />
    [Activity]
    public async Task<ProjectProvisioningResult> ProvisionResourcesAsync(
        Guid projectId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Provisioning resources for project {ProjectId}", projectId);

        try
        {
            // Simulate resource provisioning (database, storage, etc.)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            var resourceIds = new Dictionary<string, string>
            {
                ["database"] = $"db_{projectId:N}",
                ["storage"] = $"storage_{projectId:N}",
                ["cache"] = $"cache_{projectId:N}"
            };

            _logger.LogInformation("Successfully provisioned resources for project {ProjectId}", projectId);
            return new ProjectProvisioningResult(true, resourceIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to provision resources for project {ProjectId}", projectId);
            return new ProjectProvisioningResult(false, new Dictionary<string, string>(), ex.Message);
        }
    }

    /// <inheritdoc />
    [Activity]
    public async Task SendNotificationsAsync(
        Guid projectId,
        Guid tenantId,
        string notificationType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending {NotificationType} notifications for project {ProjectId}", notificationType, projectId);

        // Simulate email/notification sending
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        _logger.LogInformation("Notifications sent successfully for project {ProjectId}", projectId);
    }

    /// <inheritdoc />
    [Activity]
    public async Task ConfigurePermissionsAsync(
        Guid projectId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Configuring permissions for project {ProjectId}", projectId);

        try
        {
            // Example: Set up default OpenFGA relationships
            // In real implementation, this might involve:
            // - Creating organization-level permissions
            // - Setting up default role assignments
            // - Configuring resource-level access controls

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            _logger.LogInformation("Permissions configured successfully for project {ProjectId}", projectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure permissions for project {ProjectId}", projectId);
            throw;
        }
    }
}
