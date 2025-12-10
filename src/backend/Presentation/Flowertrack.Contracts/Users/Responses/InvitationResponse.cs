namespace Flowertrack.Contracts.Users.Responses;

/// <summary>
/// Response for user invitation
/// </summary>
public sealed record InvitationResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    
    /// <summary>
    /// Null when password was set by admin (no invitation needed)
    /// </summary>
    public DateTimeOffset? InvitationValidUntil { get; init; }
}
