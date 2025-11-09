using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Users.Commands.ResetPassword;

/// <summary>
/// Command to reset user password with new password
/// Uses reset token from password reset email
/// </summary>
public sealed record ResetPasswordCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// New password for the account
    /// </summary>
    public required string NewPassword { get; init; }

    /// <summary>
    /// Password reset token (from email link)
    /// </summary>
    public required string Token { get; init; }
}
