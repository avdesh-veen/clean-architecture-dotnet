using System.Security.Claims;

namespace SampleProject.Authorization;

/// <summary>
/// OpenFGA service for relationship-based access control checks.
/// </summary>
public interface IOpenFgaService
{
    /// <summary>
    /// Checks if a user has a specific relation to an object.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="relation">The relation (e.g., "viewer", "editor", "owner").</param>
    /// <param name="objectType">The object type (e.g., "project", "organization").</param>
    /// <param name="objectId">The object identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the relation exists; otherwise false.</returns>
    Task<bool> CheckAsync(
        Guid userId,
        string relation,
        string objectType,
        Guid objectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a relationship tuple to OpenFGA.
    /// </summary>
    Task WriteRelationshipAsync(
        Guid userId,
        string relation,
        string objectType,
        Guid objectId,
        CancellationToken cancellationToken = default);
}

namespace SampleProject.Authentication;

/// <summary>
/// Keycloak JWT token service for extracting user claims and tenant context.
/// </summary>
public interface IKeycloakTokenService
{
    /// <summary>
    /// Extracts user ID from JWT claims following the priority order: vitaUserId → NameIdentifier → sub → id.
    /// </summary>
    Guid? GetUserId(ClaimsPrincipal user);

    /// <summary>
    /// Extracts tenant ID from JWT claims.
    /// </summary>
    Guid? GetTenantId(ClaimsPrincipal user);

    /// <summary>
    /// Validates if the JWT token contains required claims for multi-tenant access.
    /// </summary>
    bool IsValidForTenant(ClaimsPrincipal user);
}
