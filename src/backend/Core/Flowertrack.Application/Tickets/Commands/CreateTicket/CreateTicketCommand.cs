using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Command to create a new service ticket
/// US-015: Tworzenie zgłoszenia serwisowego
/// </summary>
public sealed record CreateTicketCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Organization ID for which the ticket is created
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Machine ID related to the ticket
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Ticket title (max 200 characters)
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Detailed description of the issue (max 2000 characters)
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Ticket priority level
    /// </summary>
    public Priority Priority { get; init; }

    /// <summary>
    /// User ID who is creating the ticket
    /// </summary>
    public Guid CreatedBy { get; init; }
}
