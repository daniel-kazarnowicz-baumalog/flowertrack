namespace Flowertrack.Domain.Enums;

/// <summary>
/// Represents the operational status of a machine.
/// </summary>
public enum MachineStatus
{
    /// <summary>
    /// Machine is operational and functioning normally.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Machine is currently offline or not in use.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Machine is under maintenance.
    /// </summary>
    Maintenance = 2,

    /// <summary>
    /// Machine has an active alarm or error condition.
    /// </summary>
    Alarm = 3,

    /// <summary>
    /// Machine is retired or decommissioned.
    /// </summary>
    Retired = 4
}
