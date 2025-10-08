using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SampleProject.Application.Requests;
using SampleProject.Authentication;
using SampleProject.Authorization;

namespace SampleProject.Api.Controllers;

/// <summary>
/// API controller for managing project resources within the administration system.
/// Provides RESTful endpoints for creating, reading, updating, and deleting (CRUD) project entities.
/// This controller handles direct API requests and follows REST principles.
/// </summary>
/// <remarks>
/// This controller processes standard REST operations. Use this controller for single-entity operations
/// and basic project management tasks.
/// </remarks>
/// <param name="mediator">
/// The <see cref="IMediator"/> instance used to send MediatR requests to their corresponding handlers.
/// Responsible for dispatching commands and queries within the application.
/// </param>
/// <param name="keycloakTokenService">Service for extracting user and tenant information from Keycloak JWT tokens.</param>
/// <param name="openFgaService">Service for OpenFGA relationship-based authorization checks.</param>
[Route("api/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ProjectsController(
    IMediator mediator,
    IKeycloakTokenService keycloakTokenService,
    IOpenFgaService openFgaService) : ControllerBase
{
    private readonly ILogger _logger = Log.ForContext<ProjectsController>();

    /// <summary>
    /// Retrieves a specific project by its unique identifier (UUID).
    /// </summary>
    /// <param name="uuid" example="1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d">The unique identifier (UUID) of the project to retrieve.</param>
    /// <returns>An ActionResult containing the requested project data if found.</returns>
    /// <response code="200">Returns the specified project with complete details.</response>
    /// <response code="400">If the provided UUID format is invalid or Guid.Empty.</response>
    /// <response code="404">If no project is found with the specified UUID.</response>
    /// <remarks>
    /// This endpoint provides detailed information about a single project resource.
    ///
    /// Sample request (v1.0):
    /// <code language="http">
    /// GET /api/projects/1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d?api-version=1.0
    /// </code>
    /// </remarks>
    [HttpGet("{uuid:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetProject(Guid uuid)
    {
        var result = await mediator.Send(new GetProjectRequest(uuid));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new project resource in the system.
    /// </summary>
    /// <param name="projectRequest">The project entity details to create.</param>
    /// <returns>An ActionResult containing the newly created project with server-assigned identifiers.</returns>
    /// <response code="201">Returns the newly created project and sets the Location header to the new resource URL.</response>
    /// <response code="400">If the request body fails model validation or invalid JWT token.</response>
    /// <response code="403">If the user lacks permission to create projects in this tenant.</response>
    /// <remarks>
    /// This endpoint uses OpenFGA to verify the user has 'create' permission on 'projects' within their tenant.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProjectResponse>> CreateProject([FromBody] CreateProjectRequest projectRequest)
    {
        // Extract user and tenant from Keycloak JWT
        var userId = keycloakTokenService.GetUserId(User);
        var tenantId = keycloakTokenService.GetTenantId(User);

        if (!userId.HasValue || !tenantId.HasValue)
        {
            _logger.LogWarning("Invalid JWT token - missing user ID or tenant ID");
            return BadRequest("Invalid authentication token");
        }

        // Check OpenFGA permission: user can create projects in this tenant
        var canCreateProjects = await openFgaService.CheckAsync(
            userId.Value,
            "create",
            "projects",
            tenantId.Value);

        if (!canCreateProjects)
        {
            _logger.LogWarning(
                "User {UserId} lacks permission to create projects in tenant {TenantId}",
                userId.Value,
                tenantId.Value);
            return Forbid();
        }

        var result = await mediator.Send(projectRequest);

        // Establish OpenFGA relationship: user is owner of the created project
        await openFgaService.WriteRelationshipAsync(
            userId.Value,
            "owner",
            "project",
            result.Uuid);

        _logger.LogInformation(
            "Project {ProjectId} created by user {UserId} in tenant {TenantId}",
            result.Uuid,
            userId.Value,
            tenantId.Value);

        return CreatedAtAction(nameof(GetProject), new { uuid = result.Uuid }, result);
    }
}
