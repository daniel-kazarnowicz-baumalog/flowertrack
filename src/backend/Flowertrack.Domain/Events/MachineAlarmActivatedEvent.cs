using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an alarm is activated on a machine
/// </summary>
public sealed class MachineAlarmActivatedEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public string Reason { get; init; }
    public MachineStatus PreviousStatus { get; init; }

    public MachineAlarmActivatedEvent(Guid machineId, string reason, MachineStatus previousStatus)
    {
        MachineId = machineId;
        Reason = reason;
        PreviousStatus = previousStatus;
    }
}
