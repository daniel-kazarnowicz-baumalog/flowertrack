namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to login an organization user (operator/admin)
/// </summary>
public sealed record LoginOrganizationUserRequest
{
    /// <summary>
    /// Email address of the organization user
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Password
    /// </summary>
    public required string Password { get; init; }
}
