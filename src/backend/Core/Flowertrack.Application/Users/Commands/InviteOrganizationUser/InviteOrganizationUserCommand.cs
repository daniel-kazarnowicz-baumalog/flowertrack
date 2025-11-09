using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.InviteOrganizationUser;

/// <summary>
/// Command to invite/create a new organization user (operator or admin)
/// Organization admin invites new team members
/// </summary>
public sealed record InviteOrganizationUserCommand : IRequest<Result<InviteOrganizationUserResult>>
{
    /// <summary>
    /// Organization ID
    /// </summary>
    public required Guid OrganizationId { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// First name
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Last name
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Role (organization_admin or organization_operator)
    /// </summary>
    public required string Role { get; init; }
}

/// <summary>
/// Result of inviting organization user
/// </summary>
public sealed record InviteOrganizationUserResult
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string InvitationToken { get; init; }
    public required DateTimeOffset InvitationTokenExpiresAt { get; init; }
}
