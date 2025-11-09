using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Queries.GetTicket;

/// <summary>
/// Query to get a specific ticket by ID with full details
/// US-015: Wyświetlanie szczegółów zgłoszenia
/// </summary>
public sealed record GetTicketQuery : IRequest<Result<TicketDetailDto>>
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID requesting the ticket (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}
