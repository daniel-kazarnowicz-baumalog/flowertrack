namespace Flowertrack.Contracts.Users.Requests;

/// <summary>
/// Request to resend invitation email to a pending organization user.
/// US-051: Ponowne wysłanie zaproszenia
/// </summary>
public record ResendInviteRequest
{
    /// <summary>
    /// The organization user's ID
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// The organization ID (for verification)
    /// </summary>
    public Guid OrganizationId { get; init; }
}
