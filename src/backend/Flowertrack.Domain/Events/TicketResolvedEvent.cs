using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket is resolved
/// </summary>
public sealed class TicketResolvedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public string Reason { get; }
    public Guid ResolvedByUserId { get; }
    public DateTimeOffset ResolvedAt { get; }

    public TicketResolvedEvent(Guid ticketId, string reason, Guid resolvedByUserId, DateTimeOffset resolvedAt)
    {
        TicketId = ticketId;
        Reason = reason;
        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = resolvedAt;
    }
}
