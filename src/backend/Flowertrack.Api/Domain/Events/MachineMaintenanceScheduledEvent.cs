namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when preventive maintenance is scheduled for a machine.
/// This helps track maintenance schedules and intervals.
/// </summary>
public sealed record MachineMaintenanceScheduledEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Date when maintenance is scheduled
    /// </summary>
    public DateTimeOffset ScheduledDate { get; init; }

    /// <summary>
    /// Maintenance interval in days (for recurring maintenance)
    /// </summary>
    public int IntervalDays { get; init; }

    /// <summary>
    /// User who scheduled the maintenance
    /// </summary>
    public Guid ScheduledBy { get; init; }

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
