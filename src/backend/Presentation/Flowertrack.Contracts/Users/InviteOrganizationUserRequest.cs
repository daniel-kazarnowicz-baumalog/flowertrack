namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to invite a new organization user
/// </summary>
public sealed record InviteOrganizationUserRequest
{
    /// <summary>
    /// Organization ID
    /// </summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// First name
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Role (organization_admin or organization_operator)
    /// </summary>
    public required string Role { get; init; }
}
