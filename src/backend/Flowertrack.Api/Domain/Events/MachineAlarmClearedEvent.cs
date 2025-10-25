using Flowertrack.Api.Domain.Common;

namespace Flowertrack.Api.Domain.Events;

/// <summary>
/// Event raised when an alarm is cleared on a machine
/// </summary>
public sealed record MachineAlarmClearedEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public string Reason { get; init; }

    public MachineAlarmClearedEvent(Guid machineId, string reason)
    {
        MachineId = machineId;
        Reason = reason;
    }
}
