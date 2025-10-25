namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a ticket is closed.
/// This marks the final state of the ticket lifecycle (unless reopened).
/// </summary>
public sealed class TicketClosedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; }

    /// <summary>
    /// User who closed the ticket
    /// </summary>
    public Guid ClosedBy { get; }

    /// <summary>
    /// When the ticket was closed
    /// </summary>
    public DateTimeOffset ClosedAt { get; }

    /// <summary>
    /// Note explaining the closure or final outcome
    /// </summary>
    public string ClosureNote { get; }

    public TicketClosedEvent(
        Guid ticketId,
        Guid closedBy,
        DateTimeOffset closedAt,
        string closureNote)
        : base(ticketId)
    {
        TicketId = ticketId;
        ClosedBy = closedBy;
        ClosedAt = closedAt;
        ClosureNote = closureNote;
    }
}
