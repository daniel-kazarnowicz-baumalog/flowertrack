using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Event raised when a machine's API token is generated or regenerated
/// </summary>
public sealed class MachineApiTokenGeneratedEvent : DomainEvent
{
    public Guid MachineId { get; init; }
    public bool IsRegeneration { get; init; }
    public string Reason { get; init; }

    public MachineApiTokenGeneratedEvent(Guid machineId, bool isRegeneration, string reason)
    {
        MachineId = machineId;
        IsRegeneration = isRegeneration;
        Reason = reason;
    }
}
