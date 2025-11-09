using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Command to update an existing ticket's basic information
/// US-015: Edycja zgłoszenia serwisowego
/// </summary>
public sealed record UpdateTicketCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Ticket ID to update
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Updated title (optional - only update if provided)
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Updated description (optional - only update if provided)
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated priority (optional - only update if provided)
    /// </summary>
    public Priority? Priority { get; init; }

    /// <summary>
    /// User ID performing the update
    /// </summary>
    public Guid UpdatedBy { get; init; }
}
