namespace Flowertrack.Application.Tickets.Queries.GetTicketComments;

/// <summary>
/// DTO for ticket comment
/// </summary>
public sealed record CommentDto
{
    /// <summary>
    /// Comment ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Ticket ID
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// User ID who created the comment
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// User name who created the comment (denormalized for display)
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is an internal comment (visible only to service team)
    /// </summary>
    public bool IsInternal { get; init; }

    /// <summary>
    /// When the comment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the comment was last updated (null if never updated)
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
