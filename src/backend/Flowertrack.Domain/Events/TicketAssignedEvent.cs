using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket is assigned to a user
/// </summary>
public sealed class TicketAssignedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public Guid AssignedToUserId { get; }
    public Guid AssignedByUserId { get; }

    public TicketAssignedEvent(Guid ticketId, Guid assignedToUserId, Guid assignedByUserId)
    {
        TicketId = ticketId;
        AssignedToUserId = assignedToUserId;
        AssignedByUserId = assignedByUserId;
    }
}
