namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// User activation status
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account pending activation
    /// </summary>
    PendingActivation = 0,

    /// <summary>
    /// User account is active
    /// </summary>
    Active = 1,

    /// <summary>
    /// User account is inactive/deactivated
    /// </summary>
    Inactive = 2
}
