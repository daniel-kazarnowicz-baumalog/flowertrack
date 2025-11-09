using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.UpdateTicketComment;

/// <summary>
/// Command to update a comment on a ticket
/// </summary>
public sealed record UpdateTicketCommentCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Comment ID to update
    /// </summary>
    public Guid CommentId { get; init; }

    /// <summary>
    /// New comment content (max 5000 characters)
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// User ID who is updating the comment (must be the author)
    /// </summary>
    public Guid UserId { get; init; }
}
