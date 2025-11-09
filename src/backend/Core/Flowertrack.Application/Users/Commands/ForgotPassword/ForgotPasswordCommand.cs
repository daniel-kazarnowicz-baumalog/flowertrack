using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ForgotPassword;

/// <summary>
/// Command to request password reset email
/// Triggers Supabase to send password reset email to user
/// </summary>
public sealed record ForgotPasswordCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Email address of the user requesting password reset
    /// </summary>
    public required string Email { get; init; }
}
