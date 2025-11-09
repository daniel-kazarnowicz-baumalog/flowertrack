using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Raised when a comment is soft-deleted
/// </summary>
public sealed class TicketCommentDeletedEvent : DomainEvent
{
    public TicketCommentDeletedEvent(
        Guid commentId,
        Guid ticketId,
        Guid deletedBy,
        DateTimeOffset occurredAt) : base(ticketId)
    {
        CommentId = commentId;
        TicketId = ticketId;
        DeletedBy = deletedBy;
    }

    public Guid CommentId { get; }
    public Guid TicketId { get; }
    public Guid DeletedBy { get; }
}
