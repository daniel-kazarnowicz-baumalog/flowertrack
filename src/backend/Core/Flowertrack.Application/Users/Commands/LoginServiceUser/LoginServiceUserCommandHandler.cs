using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.LoginServiceUser;

/// <summary>
/// Handler for LoginServiceUserCommand
/// Authenticates service user via Supabase Auth and retrieves domain entity
/// </summary>
public sealed class LoginServiceUserCommandHandler 
    : IRequestHandler<LoginServiceUserCommand, Result<AuthResult>>
{
    private readonly IAuthService _authService;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ILogger<LoginServiceUserCommandHandler> _logger;

    public LoginServiceUserCommandHandler(
        IAuthService authService,
        IServiceUserRepository serviceUserRepository,
        ILogger<LoginServiceUserCommandHandler> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _serviceUserRepository = serviceUserRepository ?? throw new ArgumentNullException(nameof(serviceUserRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<AuthResult>> Handle(
        LoginServiceUserCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to login service user: {Email}", request.Email);

            // Authenticate via Supabase Auth
            var authResult = await _authService.SignInAsync(
                request.Email, 
                request.Password, 
                cancellationToken);

            if (!authResult.Success || authResult.User == null)
            {
                _logger.LogWarning("Login failed for service user: {Email}", request.Email);
                return Result.Failure<AuthResult>("Invalid email or password");
            }

            // Get ServiceUser entity from database by SupabaseUserId
            var serviceUser = await _serviceUserRepository.GetBySupabaseUserIdAsync(
                Guid.Parse(authResult.User.Id!), 
                cancellationToken);

            if (serviceUser == null)
            {
                _logger.LogWarning(
                    "ServiceUser entity not found for Supabase User ID: {SupabaseUserId}", 
                    authResult.User.Id);
                return Result.Failure<AuthResult>("Service user not found. Please contact administrator.");
            }

            // Verify user is active
            if (serviceUser.Status != Domain.Enums.UserStatus.Active)
            {
                _logger.LogWarning(
                    "Login attempt by inactive service user: {UserId}, Status: {Status}", 
                    serviceUser.Id, 
                    serviceUser.Status);
                return Result.Failure<AuthResult>("Your account is not active. Please contact administrator.");
            }

            _logger.LogInformation(
                "Successfully logged in service user: {Email}, ID: {UserId}", 
                request.Email, 
                serviceUser.Id);

            return Result.Success<AuthResult>(authResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during service user login: {Email}", request.Email);
            return Result.Failure<AuthResult>("An error occurred during login. Please try again.");
        }
    }
}
