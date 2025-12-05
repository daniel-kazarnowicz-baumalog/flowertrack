namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to add a comment to a ticket
/// </summary>
public sealed record AddCommentRequest
{
    /// <summary>
    /// Comment content (max 5000 characters)
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is an internal comment (visible only to service team)
    /// Only service users can create internal comments
    /// </summary>
    public bool IsInternal { get; init; }
}
