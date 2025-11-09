using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.ActivateOrganizationUser;

/// <summary>
/// Handler for ActivateOrganizationUserCommand
/// Validates invitation token and activates organization user account
/// </summary>
public sealed class ActivateOrganizationUserCommandHandler
    : IRequestHandler<ActivateOrganizationUserCommand, Result<ActivateOrganizationUserResult>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<ActivateOrganizationUserCommandHandler> _logger;

    public ActivateOrganizationUserCommandHandler(
        IOrganizationUserRepository organizationUserRepository,
        IAuthService authService,
        ILogger<ActivateOrganizationUserCommandHandler> logger)
    {
        _organizationUserRepository = organizationUserRepository;
        _authService = authService;
        _logger = logger;
    }

    public async Task<Result<ActivateOrganizationUserResult>> Handle(
        ActivateOrganizationUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing account activation request");

        // 1. Find user by invitation token
        var user = await _organizationUserRepository.GetByInvitationTokenAsync(
            request.Token,
            cancellationToken
        );

        if (user is null)
        {
            _logger.LogWarning("Invalid activation token: {Token}", request.Token);
            return Result.Failure<ActivateOrganizationUserResult>(
                "Invalid or expired activation token"
            );
        }

        // 2. Validate token hasn't expired
        if (!user.IsInvitationTokenValid(request.Token))
        {
            _logger.LogWarning(
                "Expired activation token for user: {UserId}",
                user.Id
            );
            return Result.Failure<ActivateOrganizationUserResult>(
                "Activation token has expired. Please request a new invitation."
            );
        }

        // 3. If password provided, update in Supabase
        if (!string.IsNullOrWhiteSpace(request.Password) && user.SupabaseUserId.HasValue)
        {
            try
            {
                // Note: This requires admin access token or the user's current session
                // For now, we'll skip password update during activation
                // Password should be set via Supabase email confirmation flow
                _logger.LogInformation(
                    "Password update during activation not implemented - user should set via Supabase email"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update password during activation");
                // Continue with activation even if password update fails
            }
        }

        // 4. Activate the account
        user.Activate();

        await _organizationUserRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation(
            "Account activated successfully for user: {UserId}, Email: {Email}",
            user.Id,
            user.Email.Value
        );

        return Result.Success(new ActivateOrganizationUserResult
        {
            UserId = user.Id,
            Email = user.Email.Value,
            FullName = $"{user.FirstName} {user.LastName}",
            OrganizationId = user.OrganizationId,
            Message = "Account activated successfully. You can now log in."
        });
    }
}
