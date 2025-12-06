using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.AbandonTicket;

/// <summary>
/// Command to abandon a ticket in Draft or New status
/// US-043: Porzucanie zgłoszeń
/// </summary>
public sealed record AbandonTicketCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Ticket ID to abandon
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Reason for abandonment (optional)
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// User ID performing the abandonment
    /// </summary>
    public Guid AbandonedBy { get; init; }
}
