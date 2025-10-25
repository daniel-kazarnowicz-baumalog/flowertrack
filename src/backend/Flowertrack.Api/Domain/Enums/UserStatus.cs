namespace Flowertrack.Api.Domain.Enums;

/// <summary>
/// Represents the status of a user account
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is pending activation
    /// </summary>
    Pending = 0,

    /// <summary>
    /// User account is active and can use the system
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is temporarily inactive
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// User account has been deactivated
    /// </summary>
    Deactivated = 3
}
