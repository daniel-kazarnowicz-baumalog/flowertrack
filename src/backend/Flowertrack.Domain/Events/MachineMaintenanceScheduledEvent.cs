using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when preventive maintenance is scheduled for a machine.
/// This helps track maintenance schedules and intervals.
/// </summary>
public sealed class MachineMaintenanceScheduledEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// Date when maintenance is scheduled
    /// </summary>
    public DateTimeOffset ScheduledDate { get; }

    /// <summary>
    /// Maintenance interval in days (for recurring maintenance)
    /// </summary>
    public int IntervalDays { get; }

    /// <summary>
    /// User who scheduled the maintenance
    /// </summary>
    public Guid ScheduledBy { get; }

    public MachineMaintenanceScheduledEvent(
        Guid machineId,
        DateTimeOffset scheduledDate,
        int intervalDays,
        Guid scheduledBy)
        : base(machineId)
    {
        MachineId = machineId;
        ScheduledDate = scheduledDate;
        IntervalDays = intervalDays;
        ScheduledBy = scheduledBy;
    }
}
