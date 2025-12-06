using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ResendOrganizationUserInvite;

/// <summary>
/// Command to resend invitation email to a pending organization user.
/// US-051: Ponowne wysłanie zaproszenia
/// </summary>
public record ResendOrganizationUserInviteCommand : IRequest<Result<ResendInviteResult>>
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

/// <summary>
/// Result of resending an invitation
/// </summary>
public record ResendInviteResult
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
    /// Message for the frontend
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
