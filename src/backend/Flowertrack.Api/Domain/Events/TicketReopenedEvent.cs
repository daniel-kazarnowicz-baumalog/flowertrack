namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a previously closed ticket is reopened.
/// This typically happens when an issue recurs or was not fully resolved.
/// In MVP, clients can reopen tickets within 14 days of resolution.
/// </summary>
public sealed record TicketReopenedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User who reopened the ticket
    /// </summary>
    public Guid ReopenedBy { get; init; }

    /// <summary>
    /// When the ticket was reopened
    /// </summary>
    public DateTimeOffset ReopenedAt { get; init; }

    /// <summary>
    /// Reason for reopening the ticket
    /// </summary>
    public string Reason { get; init; }

    public TicketReopenedEvent(
        Guid ticketId,
        Guid reopenedBy,
        DateTimeOffset reopenedAt,
        string reason)
        : base(ticketId)
    {
        TicketId = ticketId;
        ReopenedBy = reopenedBy;
        ReopenedAt = reopenedAt;
        Reason = reason;
    }
}
