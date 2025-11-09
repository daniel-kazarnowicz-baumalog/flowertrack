using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketStatus;

/// <summary>
/// Command to update a ticket's status with state transition validation
/// </summary>
/// <remarks>
/// Valid state transitions:
/// - New → Accepted, New → Closed
/// - Accepted → InProgress, Accepted → Closed
/// - InProgress → Resolved, InProgress → Closed (with reason)
/// - Resolved → Closed, Resolved → Reopened (within 14 days)
/// - Reopened → InProgress, Reopened → Resolved, Reopened → Closed
/// - Closed → (no transitions, final state)
/// </remarks>
public sealed record UpdateTicketStatusCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// ID of the ticket to update
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// New status to transition to
    /// </summary>
    public TicketStatus NewStatus { get; init; }

    /// <summary>
    /// Reason for the status change (required for Resolved and Closed transitions)
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User performing the status change
    /// </summary>
    public Guid ChangedBy { get; init; }

    public UpdateTicketStatusCommand(Guid ticketId, TicketStatus newStatus, string? reason, Guid changedBy)
    {
        TicketId = ticketId;
        NewStatus = newStatus;
        Reason = reason;
        ChangedBy = changedBy;
    }
}
