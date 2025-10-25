namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a ticket is closed.
/// This marks the final state of the ticket lifecycle (unless reopened).
/// </summary>
public sealed record TicketClosedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User who closed the ticket
    /// </summary>
    public Guid ClosedBy { get; init; }

    /// <summary>
    /// When the ticket was closed
    /// </summary>
    public DateTimeOffset ClosedAt { get; init; }

    /// <summary>
    /// Note explaining the closure or final outcome
    /// </summary>
    public string ClosureNote { get; init; }

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
