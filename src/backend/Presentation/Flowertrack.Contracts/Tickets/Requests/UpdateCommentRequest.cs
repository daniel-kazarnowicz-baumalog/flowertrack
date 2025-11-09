namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to update a comment on a ticket
/// </summary>
public sealed record UpdateCommentRequest
{
    /// <summary>
    /// New comment content (max 5000 characters)
    /// </summary>
    public string Content { get; init; } = string.Empty;
}
