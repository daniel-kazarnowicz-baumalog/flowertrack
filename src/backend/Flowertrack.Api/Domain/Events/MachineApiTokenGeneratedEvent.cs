namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when an API token is generated for a machine.
/// This allows the machine to authenticate and send logs/data to the system.
/// Note: The actual token value is not stored in this event for security reasons.
/// </summary>
public sealed record MachineApiTokenGeneratedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// When the token was generated
    /// </summary>
    public DateTimeOffset TokenGeneratedAt { get; init; }

    /// <summary>
    /// User who generated the token
    /// </summary>
    public Guid GeneratedBy { get; init; }

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
