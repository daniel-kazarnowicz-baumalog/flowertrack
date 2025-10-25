namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a ticket is assigned to a service technician.
/// This event tracks who assigned the ticket and to whom, along with any previous assignee.
/// </summary>
public sealed record TicketAssignedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID of the technician the ticket was assigned to
    /// </summary>
    public Guid AssignedToUserId { get; init; }

    /// <summary>
    /// User who performed the assignment
    /// </summary>
    public Guid AssignedBy { get; init; }

    /// <summary>
    /// Previous assignee if the ticket was reassigned (null if first assignment)
    /// </summary>
    public Guid? PreviousAssignee { get; init; }

    public TicketAssignedEvent(
        Guid ticketId,
        Guid assignedToUserId,
        Guid assignedBy,
        Guid? previousAssignee = null)
        : base(ticketId)
    {
        TicketId = ticketId;
        AssignedToUserId = assignedToUserId;
        AssignedBy = assignedBy;
        PreviousAssignee = previousAssignee;
    }
}
