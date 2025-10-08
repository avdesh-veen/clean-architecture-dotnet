using MediatR;

namespace SampleProject.Application.Requests;

/// <summary>
/// Request to get a Project by UUID.
/// </summary>
/// <param name="Uuid">Project UUID.</param>
public record GetProjectRequest(Guid Uuid) : IRequest<ProjectResponse?>;
