using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.AddComment;

/// <summary>
/// Command to add a comment to a ticket
/// </summary>
public sealed record AddCommentCommand(
    Guid TicketId,
    string Content,
    Guid UserId,
    string UserName,
    string UserType,
    bool IsInternal = false
) : IRequest<Result<Guid>>;
