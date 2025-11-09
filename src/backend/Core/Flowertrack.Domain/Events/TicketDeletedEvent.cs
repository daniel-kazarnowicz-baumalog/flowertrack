using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Domain event raised when a ticket is deleted (soft delete)
/// </summary>
public sealed class TicketDeletedEvent : DomainEvent
{
    public Guid TicketId { get; }
    public Guid DeletedBy { get; }
    public string? Reason { get; }
    public DateTimeOffset DeletedAt { get; }

    public TicketDeletedEvent(
        Guid ticketId,
        Guid deletedBy,
        string? reason,
        DateTimeOffset deletedAt)
    {
        TicketId = ticketId;
        DeletedBy = deletedBy;
        Reason = reason;
        DeletedAt = deletedAt;
    }
}
