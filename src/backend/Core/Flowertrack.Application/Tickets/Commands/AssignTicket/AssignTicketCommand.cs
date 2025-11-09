using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.AssignTicket;

/// <summary>
/// Command to assign a ticket to a service technician
/// </summary>
public sealed record AssignTicketCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// ID of the ticket to assign
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID of the service technician to assign the ticket to
    /// </summary>
    public Guid AssignedToUserId { get; init; }

    /// <summary>
    /// User ID of the person performing the assignment (usually a manager or admin)
    /// </summary>
    public Guid AssignedBy { get; init; }

    public AssignTicketCommand(Guid ticketId, Guid assignedToUserId, Guid assignedBy)
    {
        TicketId = ticketId;
        AssignedToUserId = assignedToUserId;
        AssignedBy = assignedBy;
    }
}
