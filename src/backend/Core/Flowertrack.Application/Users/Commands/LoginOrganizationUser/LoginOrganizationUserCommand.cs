using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.LoginOrganizationUser;

/// <summary>
/// Command to login an organization user (operator or admin)
/// Validates credentials via Supabase Auth and checks account activation status
/// </summary>
public sealed record LoginOrganizationUserCommand : IRequest<Result<LoginOrganizationUserResult>>
{
    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Password
    /// </summary>
    public required string Password { get; init; }
}

/// <summary>
/// Result of organization user login
/// </summary>
public sealed record LoginOrganizationUserResult
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required Guid UserId { get; init; }
    public required Guid OrganizationId { get; init; }
    public required string OrganizationName { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
    public required string Status { get; init; }
    public required bool IsActivated { get; init; }
}
