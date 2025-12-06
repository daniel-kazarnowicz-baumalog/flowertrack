namespace Flowertrack.Contracts.Users.Responses;

/// <summary>
/// Response for resend invitation request.
/// US-051: Ponowne wysłanie zaproszenia
/// </summary>
public record ResendInviteResponse
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// The user's email
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// When the new invitation token expires
    /// </summary>
    public DateTimeOffset InvitationTokenExpiresAt { get; init; }

    /// <summary>
    /// Success message
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
