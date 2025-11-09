using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ActivateOrganizationUser;

/// <summary>
/// Command to activate organization user account
/// Sets IsActivated = true after email confirmation
/// </summary>
public sealed record ActivateOrganizationUserCommand : IRequest<Result<ActivateOrganizationUserResult>>
{
    /// <summary>
    /// Activation token (from invitation email or Supabase confirmation)
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Optional: Initial password to set during activation
    /// If provided, updates Supabase Auth password
    /// </summary>
    public string? Password { get; init; }
}

/// <summary>
/// Result of account activation
/// </summary>
public sealed record ActivateOrganizationUserResult
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required Guid OrganizationId { get; init; }
    public string Message { get; init; } = "Account activated successfully. You can now log in.";
}
