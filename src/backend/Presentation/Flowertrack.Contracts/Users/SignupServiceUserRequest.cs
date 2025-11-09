namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to create/signup a new service user (technician)
/// Admin-initiated action
/// </summary>
public sealed record SignupServiceUserRequest
{
    /// <summary>
    /// Email address of the service user
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
    /// Specialization/role description (optional)
    /// </summary>
    public string? Specialization { get; init; }

    /// <summary>
    /// Initial password for the account
    /// If not provided, user will need to set password via activation link
    /// </summary>
    public string? Password { get; init; }
}
