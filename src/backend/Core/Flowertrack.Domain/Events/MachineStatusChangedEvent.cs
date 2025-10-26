using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a machine's operational status changes.
/// This tracks changes in machine state (e.g., Active, Inactive, Maintenance, Decommissioned).
/// </summary>
public sealed class MachineStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the machine
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// Previous status before the change
    /// </summary>
    public string OldStatus { get; }

    /// <summary>
    /// New status after the change
    /// </summary>
    public string NewStatus { get; }

    /// <summary>
    /// Reason for the status change
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// When the status was changed
    /// </summary>
    public DateTimeOffset ChangedAt { get; }

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
