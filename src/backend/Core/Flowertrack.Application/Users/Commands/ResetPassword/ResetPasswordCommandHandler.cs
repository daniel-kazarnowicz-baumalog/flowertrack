using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.ResetPassword;

/// <summary>
/// Handler for ResetPasswordCommand
/// Resets user password via Supabase Auth using reset token
/// </summary>
public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly IAuthService _authService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IAuthService authService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing password reset request");

        try
        {
            // Reset password via Supabase
            // Note: The access token from password reset flow must be provided
            // Frontend should extract it from the reset link and pass it here
            var success = await _authService.UpdatePasswordAsync(
                request.Token, // This should be the access token from reset link
                request.NewPassword,
                cancellationToken
            );

            if (!success)
            {
                _logger.LogWarning("Password reset failed - invalid or expired token");
                return Result.Failure<bool>(
                    "Failed to reset password. The reset link may have expired or is invalid."
                );
            }

            _logger.LogInformation("Password reset successful");

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to reset password"
            );

            return Result.Failure<bool>(
                "Failed to reset password. The reset link may have expired or is invalid."
            );
        }
    }
}
