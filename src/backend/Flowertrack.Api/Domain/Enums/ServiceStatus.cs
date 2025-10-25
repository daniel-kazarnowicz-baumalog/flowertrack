namespace Flowertrack.Api.Domain.Enums;

/// <summary>
/// Represents the service status of an organization
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// Organization is active and can use all services
    /// </summary>
    Active = 0,

    /// <summary>
    /// Organization service is temporarily suspended
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// Organization contract has expired
    /// </summary>
    Expired = 2,

    /// <summary>
    /// Organization is in trial period
    /// </summary>
    Trial = 3,

    /// <summary>
    /// Organization account is inactive
    /// </summary>
    Inactive = 4
}
