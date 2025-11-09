namespace Flowertrack.Contracts.Users;

/// <summary>
/// Response after successful organization user login
/// </summary>
public sealed record LoginOrganizationUserResponse
{
    /// <summary>
    /// JWT access token from Supabase
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Organization user information
    /// </summary>
    public required OrganizationUserDto User { get; init; }
}

/// <summary>
/// DTO for organization user details
/// </summary>
public sealed record OrganizationUserDto
{
    /// <summary>
    /// User ID (domain entity ID)
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Organization ID
    /// </summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization name
    /// </summary>
    public required string OrganizationName { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Full name (first + last)
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// User role (organization_admin, organization_operator)
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// Account status
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Is account activated
    /// </summary>
    public required bool IsActivated { get; init; }
}
