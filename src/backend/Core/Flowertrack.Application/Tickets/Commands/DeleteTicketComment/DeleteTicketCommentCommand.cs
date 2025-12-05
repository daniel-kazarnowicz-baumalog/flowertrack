using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.DeleteTicketComment;

/// <summary>
/// Command to delete a comment from a ticket (soft delete)
/// </summary>
public sealed record DeleteTicketCommentCommand : IRequest<Result>
{
    /// <summary>
    /// Comment ID to delete
    /// </summary>
    public Guid CommentId { get; init; }

    /// <summary>
    /// User ID who is deleting the comment (must be the author)
    /// </summary>
    public Guid UserId { get; init; }
}
