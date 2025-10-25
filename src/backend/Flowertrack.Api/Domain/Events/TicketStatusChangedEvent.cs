namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a ticket's status changes.
/// This event captures the transition from one status to another along with the reason for the change.
/// </summary>
public sealed record TicketStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Previous status before the change
    /// </summary>
    public string OldStatus { get; init; }

    /// <summary>
    /// New status after the change
    /// </summary>
    public string NewStatus { get; init; }

    /// <summary>
    /// Reason or justification for the status change
    /// </summary>
    public string Reason { get; init; }

    /// <summary>
    /// User who changed the status
    /// </summary>
    public Guid ChangedBy { get; init; }

    /// <summary>
    /// When the status was changed
    /// </summary>
    public DateTimeOffset ChangedAt { get; init; }

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
