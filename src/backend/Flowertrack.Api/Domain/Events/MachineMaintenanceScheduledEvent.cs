using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when maintenance is scheduled for a machine
/// </summary>
public sealed record MachineMaintenanceScheduledEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public DateOnly ScheduledDate { get; init; }
    public int? MaintenanceIntervalId { get; init; }

    public MachineMaintenanceScheduledEvent(Guid machineId, DateOnly scheduledDate, int? maintenanceIntervalId)
    {
        MachineId = machineId;
        ScheduledDate = scheduledDate;
        MaintenanceIntervalId = maintenanceIntervalId;
    }
}
