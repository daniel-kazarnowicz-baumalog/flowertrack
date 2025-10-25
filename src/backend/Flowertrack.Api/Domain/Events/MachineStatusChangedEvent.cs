namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a machine's operational status changes.
/// This tracks changes in machine state (e.g., Active, Inactive, Maintenance, Decommissioned).
/// </summary>
public sealed record MachineStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Previous status before the change
    /// </summary>
    public string OldStatus { get; init; }

    /// <summary>
    /// New status after the change
    /// </summary>
    public string NewStatus { get; init; }

    /// <summary>
    /// Reason for the status change
    /// </summary>
    public string Reason { get; init; }

    /// <summary>
    /// When the status was changed
    /// </summary>
    public DateTimeOffset ChangedAt { get; init; }

    public MachineStatusChangedEvent(
        Guid machineId,
        string oldStatus,
        string newStatus,
        string reason,
        DateTimeOffset changedAt)
        : base(machineId)
    {
        MachineId = machineId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedAt = changedAt;
    }
}
