namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Service status for organizations
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// Service contract is active
    /// </summary>
    Active = 1,

    /// <summary>
    /// Service temporarily suspended
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Service contract expired
    /// </summary>
    Expired = 3
}
