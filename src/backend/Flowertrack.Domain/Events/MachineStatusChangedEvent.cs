using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when a machine's status changes
/// </summary>
public sealed class MachineStatusChangedEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public MachineStatus PreviousStatus { get; init; }
    public MachineStatus NewStatus { get; init; }
    public string Reason { get; init; }

    public MachineStatusChangedEvent(Guid machineId, MachineStatus previousStatus, MachineStatus newStatus, string reason)
    {
        MachineId = machineId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}
