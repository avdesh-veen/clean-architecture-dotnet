using Temporalio.Workflows;

namespace SampleProject.Worker.Temporal;

/// <summary>
/// Temporal workflow interface for project lifecycle management.
/// Handles long-running processes like provisioning, notifications, and cleanup.
/// </summary>
[Workflow("project-lifecycle")]
public interface IProjectWorkflow
{
    /// <summary>
    /// Runs the complete project provisioning workflow.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>Workflow execution task.</returns>
    [WorkflowRun]
    Task RunAsync(Guid projectId, Guid tenantId);

    /// <summary>
    /// Signals the workflow to update project configuration.
    /// </summary>
    /// <param name="configuration">New configuration data.</param>
    [WorkflowSignal]
    Task UpdateConfigurationAsync(string configuration);

    /// <summary>
    /// Queries the current workflow status.
    /// </summary>
    /// <returns>The current workflow status.</returns>
    [WorkflowQuery]
    string GetStatus();
}

/// <summary>
/// Temporal activities interface for project workflow operations.
/// Activities are deterministic functions that interact with external systems.
/// </summary>
public interface IProjectActivities
{
    /// <summary>
    /// Provisions project resources (databases, storage, etc.).
    /// </summary>
    Task<ProjectProvisioningResult> ProvisionResourcesAsync(
        Guid projectId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends notification emails to project stakeholders.
    /// </summary>
    Task SendNotificationsAsync(
        Guid projectId,
        Guid tenantId,
        string notificationType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures project permissions and access control.
    /// </summary>
    Task ConfigurePermissionsAsync(
        Guid projectId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of project provisioning activity.
/// </summary>
/// <param name="Success">Whether provisioning succeeded.</param>
/// <param name="ResourceIds">Identifiers of created resources.</param>
/// <param name="ErrorMessage">Error message if provisioning failed.</param>
public record ProjectProvisioningResult(
    bool Success,
    Dictionary<string, string> ResourceIds,
    string? ErrorMessage = null);
