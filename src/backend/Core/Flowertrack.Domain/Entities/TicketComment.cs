using Flowertrack.Domain.Common;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a comment on a service ticket
/// </summary>
public sealed class TicketComment : AuditableEntity<Guid>
{
    private TicketComment() : base(Guid.NewGuid()) { } // EF Core constructor

    private TicketComment(
        Guid id,
        Guid ticketId,
        Guid userId,
        string content,
        bool isInternal) : base(id)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty", nameof(content));
        }

        if (content.Length > 5000)
        {
            throw new ArgumentException("Comment content cannot exceed 5000 characters", nameof(content));
        }

        Id = id;
        TicketId = ticketId;
        UserId = userId;
        Content = content;
        IsInternal = isInternal;
        SetCreatedAudit(userId);
    }

    /// <summary>
    /// ID of the ticket this comment belongs to
    /// </summary>
    public Guid TicketId { get; private set; }

    /// <summary>
    /// ID of the user who created the comment (can be ServiceUser or OrganizationUser)
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Comment content (max 5000 characters)
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Whether this comment is internal (visible only to service team)
    /// </summary>
    public bool IsInternal { get; private set; }

    /// <summary>
    /// Navigation property to ticket
    /// </summary>
    public Ticket Ticket { get; private set; } = null!;

    /// <summary>
    /// Factory method to create a new comment
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="userId">User ID who is creating the comment</param>
    /// <param name="content">Comment content</param>
    /// <param name="isInternal">Whether comment is internal (service team only)</param>
    /// <returns>New TicketComment instance</returns>
    public static TicketComment Create(
        Guid ticketId,
        Guid userId,
        string content,
        bool isInternal = false)
    {
        var comment = new TicketComment(
            Guid.NewGuid(),
            ticketId,
            userId,
            content,
            isInternal);

        comment.RaiseDomainEvent(new TicketCommentAddedEvent(
            comment.Id,
            comment.TicketId,
            comment.UserId,
            comment.Content,
            comment.IsInternal,
            DateTimeOffset.UtcNow));

        return comment;
    }

    /// <summary>
    /// Updates the comment content
    /// Can only be done within a time limit (e.g., 15 minutes) - enforced by application layer
    /// </summary>
    /// <param name="content">New content</param>
    /// <param name="userId">User performing the update (must match original author)</param>
    public void Update(string content, Guid userId)
    {
        if (userId != UserId)
        {
            throw new InvalidOperationException("Only the comment author can edit the comment");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty", nameof(content));
        }

        if (content.Length > 5000)
        {
            throw new ArgumentException("Comment content cannot exceed 5000 characters", nameof(content));
        }

        Content = content;
        SetUpdatedAudit(userId);

        RaiseDomainEvent(new TicketCommentUpdatedEvent(
            Id,
            TicketId,
            userId,
            content,
            DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Soft deletes the comment
    /// </summary>
    /// <param name="userId">User performing the deletion (must match original author or be service admin)</param>
    public void Delete(Guid userId)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Comment is already deleted");
        }

        SetDeletedAudit(userId);

        RaiseDomainEvent(new TicketCommentDeletedEvent(
            Id,
            TicketId,
            userId,
            DateTimeOffset.UtcNow));
    }
}
