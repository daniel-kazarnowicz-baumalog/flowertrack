using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Command to soft delete a ticket
/// US-015: Usuwanie zgłoszenia serwisowego
/// </summary>
public sealed record DeleteTicketCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Ticket ID to delete
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Reason for deletion (optional, for audit trail)
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User ID performing the deletion
    /// </summary>
    public Guid DeletedBy { get; init; }
}
