namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response for a single comment
/// </summary>
public sealed record CommentResponse
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
    /// User name who created the comment
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Comment content
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is an internal comment
    /// </summary>
    public bool IsInternal { get; init; }

    /// <summary>
    /// When the comment was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the comment was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; init; }
}
