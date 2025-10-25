using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket is closed
/// </summary>
public sealed class TicketClosedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public string Reason { get; }
    public Guid ClosedByUserId { get; }
    public DateTimeOffset ClosedAt { get; }

    public TicketClosedEvent(Guid ticketId, string reason, Guid closedByUserId, DateTimeOffset closedAt)
    {
        TicketId = ticketId;
        Reason = reason;
        ClosedByUserId = closedByUserId;
        ClosedAt = closedAt;
    }
}
