using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Tickets.Commands.AddTicketComment;

/// <summary>
/// Command to add a comment to a ticket
/// </summary>
public sealed record AddTicketCommentCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Ticket ID to add comment to
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Comment content (max 5000 characters)
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// User ID who is adding the comment
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Whether this is an internal comment (visible only to service team)
    /// </summary>
    public bool IsInternal { get; init; }
}
