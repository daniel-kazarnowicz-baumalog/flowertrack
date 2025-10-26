namespace Flowertrack.Domain.Enums;

/// <summary>
/// Represents the service status of an organization.
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// The organization's service is currently active.
    /// </summary>
    Active = 0,

    /// <summary>
    /// The organization's service has been suspended.
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// The organization's contract has expired.
    /// </summary>
    Expired = 2
}
