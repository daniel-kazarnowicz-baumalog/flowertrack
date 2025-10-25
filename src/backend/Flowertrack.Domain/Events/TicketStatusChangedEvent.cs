using Flowertrack.Domain.Common;
using Flowertrack.Domain.Enums;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket's status changes
/// </summary>
public sealed class TicketStatusChangedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public TicketStatus OldStatus { get; }
    public TicketStatus NewStatus { get; }
    public string Reason { get; }
    public Guid ChangedByUserId { get; }

    public TicketStatusChangedEvent(Guid ticketId, TicketStatus oldStatus, TicketStatus newStatus, string reason, Guid changedByUserId)
    {
        TicketId = ticketId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Reason = reason;
        ChangedByUserId = changedByUserId;
    }
}
