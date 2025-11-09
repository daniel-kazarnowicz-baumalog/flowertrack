namespace Flowertrack.Contracts.Users;

/// <summary>
/// Response after inviting organization user
/// </summary>
public sealed record InviteOrganizationUserResponse
{
    /// <summary>
    /// Created user ID
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Full name
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Invitation token for account activation
    /// </summary>
    public required string InvitationToken { get; init; }

    /// <summary>
    /// Token expiration
    /// </summary>
    public required DateTimeOffset InvitationTokenExpiresAt { get; init; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; init; } = "Invitation sent successfully. User will receive an email to activate their account.";
}
