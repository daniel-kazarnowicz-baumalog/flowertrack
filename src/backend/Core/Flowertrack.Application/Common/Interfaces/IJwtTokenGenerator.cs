using System.Security.Claims;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT access token for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="email">User email address</param>
    /// <param name="roles">User roles to include in claims</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(Guid userId, string email, IEnumerable<string> roles);

    /// <summary>
    /// Generates a cryptographically secure refresh token
    /// </summary>
    /// <returns>Base64 encoded refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the claims principal
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>ClaimsPrincipal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);
}
