namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Machine operational status
/// </summary>
public enum MachineStatus
{
    /// <summary>
    /// Machine is active and operating normally
    /// </summary>
    Active = 1,

    /// <summary>
    /// Machine is inactive/offline
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Machine is under maintenance
    /// </summary>
    Maintenance = 3,

    /// <summary>
    /// Machine has an alarm/error condition
    /// </summary>
    Alarm = 4
}
