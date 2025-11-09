using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.ForgotPassword;

/// <summary>
/// Handler for ForgotPasswordCommand
/// Sends password reset email via Supabase Auth
/// </summary>
public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly IAuthService _authService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IAuthService authService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Password reset requested for email: {Email}",
            request.Email
        );

        try
        {
            // Send password reset email via Supabase
            // Note: Supabase returns success even if email doesn't exist (security best practice)
            var success = await _authService.SendPasswordResetEmailAsync(
                request.Email,
                cancellationToken
            );

            _logger.LogInformation(
                "Password reset email sent to: {Email}",
                request.Email
            );

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset email to: {Email}",
                request.Email
            );

            return Result.Failure<bool>(
                "Failed to send password reset email. Please try again later."
            );
        }
    }
}
