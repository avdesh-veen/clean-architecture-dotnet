using AutoMapper;
using MediatR;
using SampleProject.Application.Requests;
using SampleProject.Application.Services.Interfaces;
using SampleProject.Domain.Models;
using FluentValidation;

namespace SampleProject.Application.Handlers;

/// <summary>
/// Handler for creating projects using standard patterns (validation, mapping, service layer).
/// </summary>
/// <param name="service">The project service.</param>
/// <param name="mapper">The AutoMapper instance.</param>
public class CreateProjectHandler(
    IBaseService<Project> service,
    IMapper mapper)
    : IRequestHandler<CreateProjectRequest, ProjectResponse>
{
    /// <summary>
    /// Handles the creation of a new project.
    /// </summary>
    /// <param name="request">The create project request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project response.</returns>
    public async Task<ProjectResponse> Handle(
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty
        };

        var createdProject = await service.CreateAsync(project, cancellationToken);

        // Publish Wolverine message for post-creation workflows
        // Note: In real implementation, inject IMessageBus and publish message
        // await messageBus.PublishAsync(new ProjectCreatedMessage(createdProject.Uuid, createdProject.TenantId, createdProject.Name));

        return mapper.Map<ProjectResponse>(createdProject);
    }
}

/// <summary>
/// Handler for retrieving a project by its unique identifier.
/// </summary>
/// <param name="service">The project service.</param>
/// <param name="mapper">The AutoMapper instance.</param>
public class GetProjectHandler(
    IBaseService<Project> service,
    IMapper mapper)
    : IRequestHandler<GetProjectRequest, ProjectResponse?>
{
    /// <summary>
    /// Handles the retrieval of a project.
    /// </summary>
    /// <param name="request">The get request containing the UUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project response or null if not found.</returns>
    public async Task<ProjectResponse?> Handle(
        GetProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        var project = await service.GetByUuidAsync(request.Uuid, cancellationToken);
        if (project == null) return null;

        return mapper.Map<ProjectResponse>(project);
    }
}

/// <summary>
/// FluentValidation validator for CreateProjectRequest.
/// </summary>
public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
    }
}
