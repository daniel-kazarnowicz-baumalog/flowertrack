namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a ticket's status changes.
/// This event captures the transition from one status to another along with the reason for the change.
/// </summary>
public sealed class TicketStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; }

    /// <summary>
    /// Previous status before the change
    /// </summary>
    public string OldStatus { get; }

    /// <summary>
    /// New status after the change
    /// </summary>
    public string NewStatus { get; }

    /// <summary>
    /// Reason or justification for the status change
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// User who changed the status
    /// </summary>
    public Guid ChangedBy { get; }

    /// <summary>
    /// When the status was changed
    /// </summary>
    public DateTimeOffset ChangedAt { get; }

    public TicketStatusChangedEvent(
        Guid ticketId,
        string oldStatus,
        string newStatus,
        string reason,
        Guid changedBy,
        DateTimeOffset changedAt)
        : base(ticketId)
    {
        TicketId = ticketId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedBy = changedBy;
        ChangedAt = changedAt;
    }
}
