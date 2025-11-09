using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

/// <summary>
/// Raised when a new comment is added to a ticket
/// </summary>
public sealed class TicketCommentAddedEvent : DomainEvent
{
    public TicketCommentAddedEvent(
        Guid commentId,
        Guid ticketId,
        Guid authorId,
        string content,
        bool isInternal,
        DateTimeOffset occurredAt) : base(ticketId)
    {
        CommentId = commentId;
        TicketId = ticketId;
        AuthorId = authorId;
        Content = content;
        IsInternal = isInternal;
    }

    public Guid CommentId { get; }
    public Guid TicketId { get; }
    public Guid AuthorId { get; }
    public string Content { get; }
    public bool IsInternal { get; }
}
