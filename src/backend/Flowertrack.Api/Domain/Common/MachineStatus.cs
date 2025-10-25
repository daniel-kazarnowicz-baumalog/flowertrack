namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Represents the operational status of a machine
/// </summary>
public enum MachineStatus
{
    /// <summary>
    /// Machine is registered but not yet operational
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Machine is operational and running normally
    /// </summary>
    Active = 1,

    /// <summary>
    /// Machine is currently under maintenance
    /// </summary>
    Maintenance = 2,

    /// <summary>
    /// Machine has an active alarm/error condition
    /// </summary>
    Alarm = 3,

    /// <summary>
    /// Machine is out of service/decommissioned
    /// </summary>
    OutOfService = 4
}
