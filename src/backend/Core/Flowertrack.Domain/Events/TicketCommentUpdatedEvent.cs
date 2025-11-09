using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Raised when an existing comment is updated
/// </summary>
public sealed class TicketCommentUpdatedEvent : DomainEvent
{
    public TicketCommentUpdatedEvent(
        Guid commentId,
        Guid ticketId,
        Guid updatedBy,
        string newContent,
        DateTimeOffset occurredAt) : base(ticketId)
    {
        CommentId = commentId;
        TicketId = ticketId;
        UpdatedBy = updatedBy;
        NewContent = newContent;
    }

    public Guid CommentId { get; }
    public Guid TicketId { get; }
    public Guid UpdatedBy { get; }
    public string NewContent { get; }
}
