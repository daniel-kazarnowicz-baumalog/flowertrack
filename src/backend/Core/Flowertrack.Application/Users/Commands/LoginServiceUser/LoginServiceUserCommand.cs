using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.LoginServiceUser;

/// <summary>
/// Command to login a service user via Supabase Auth
/// </summary>
public sealed record LoginServiceUserCommand : IRequest<Result<AuthResult>>
{
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
