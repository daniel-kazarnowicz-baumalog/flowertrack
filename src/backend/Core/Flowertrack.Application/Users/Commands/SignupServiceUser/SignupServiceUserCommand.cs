using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.SignupServiceUser;

/// <summary>
/// Command to create/signup a new service user (technician) via Supabase Auth
/// Admin-initiated action - creates account in Supabase and domain
/// </summary>
public sealed record SignupServiceUserCommand : IRequest<Result<SignupServiceUserResult>>
{
    /// <summary>
    /// Email address of the service user
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
    /// Specialization/role description (optional)
    /// </summary>
    public string? Specialization { get; init; }

    /// <summary>
    /// Initial password for the account
    /// If not provided, user will need to set password via activation link
    /// </summary>
    public string? Password { get; init; }
}

/// <summary>
/// Result of signup operation
/// </summary>
public sealed record SignupServiceUserResult
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public string? ActivationToken { get; init; }
    public DateTimeOffset? ActivationTokenExpiresAt { get; init; }
}
