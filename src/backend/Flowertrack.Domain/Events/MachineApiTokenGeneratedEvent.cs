using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when an API token is generated for a machine.
/// This allows the machine to authenticate and send logs/data to the system.
/// Note: The actual token value is not stored in this event for security reasons.
/// </summary>
public sealed class MachineApiTokenGeneratedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// When the token was generated
    /// </summary>
    public DateTimeOffset TokenGeneratedAt { get; }

    /// <summary>
    /// User who generated the token
    /// </summary>
    public Guid GeneratedBy { get; }

    public MachineApiTokenGeneratedEvent(
        Guid machineId,
        DateTimeOffset tokenGeneratedAt,
        Guid generatedBy)
        : base(machineId)
    {
        MachineId = machineId;
        TokenGeneratedAt = tokenGeneratedAt;
        GeneratedBy = generatedBy;
    }
}
