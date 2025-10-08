using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace SampleProject.Authentication;

/// <summary>
/// Keycloak token service implementation.
/// </summary>
public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly ILogger<KeycloakTokenService> _logger;

    public KeycloakTokenService(ILogger<KeycloakTokenService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Guid? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("vitaUserId")?.Value ??
                          user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          user.FindFirst("sub")?.Value ??
                          user.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("No user ID claim found in JWT token");
            return null;
        }

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        _logger.LogWarning("User ID claim {UserIdClaim} is not a valid GUID", userIdClaim);
        return null;
    }

    /// <inheritdoc />
    public Guid? GetTenantId(ClaimsPrincipal user)
    {
        var tenantIdClaim = user.FindFirst("tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim))
        {
            _logger.LogWarning("No tenant ID claim found in JWT token");
            return null;
        }

        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }

        _logger.LogWarning("Tenant ID claim {TenantIdClaim} is not a valid GUID", tenantIdClaim);
        return null;
    }

    /// <inheritdoc />
    public bool IsValidForTenant(ClaimsPrincipal user)
    {
        return GetUserId(user).HasValue && GetTenantId(user).HasValue;
    }
}
