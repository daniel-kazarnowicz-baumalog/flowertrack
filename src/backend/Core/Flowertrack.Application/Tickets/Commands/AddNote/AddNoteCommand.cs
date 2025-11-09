using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.AddNote;

/// <summary>
/// Command to add an internal note to a ticket (visible only to service users)
/// </summary>
public sealed record AddNoteCommand(
    Guid TicketId,
    string Content,
    Guid UserId,
    string UserName,
    string UserType
) : IRequest<Result<Guid>>;
