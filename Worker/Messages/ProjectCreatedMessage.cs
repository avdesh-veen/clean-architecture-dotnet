namespace SampleProject.Worker.Messages;

/// <summary>
/// Message published when a project is created.
/// </summary>
public record ProjectCreatedMessage(Guid ProjectId, Guid TenantId, string Name);
