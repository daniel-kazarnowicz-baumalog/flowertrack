using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when an alarm is cleared on a machine
/// </summary>
public sealed class MachineAlarmClearedEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public string Reason { get; init; }

    public MachineAlarmClearedEvent(Guid machineId, string reason)
    {
        MachineId = machineId;
        Reason = reason;
    }
}
