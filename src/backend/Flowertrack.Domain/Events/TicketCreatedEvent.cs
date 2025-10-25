using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket is created
/// </summary>
public sealed class TicketCreatedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public string Title { get; }
    public Guid OrganizationId { get; }
    public Guid CreatedByUserId { get; }

    public TicketCreatedEvent(Guid ticketId, string ticketNumber, string title, Guid organizationId, Guid createdByUserId)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        Title = title;
        OrganizationId = organizationId;
        CreatedByUserId = createdByUserId;
    }
}
