using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.LoginOrganizationUser;

/// <summary>
/// Handler for LoginOrganizationUserCommand
/// Authenticates organization user via Supabase and validates activation status
/// </summary>
public sealed class LoginOrganizationUserCommandHandler
    : IRequestHandler<LoginOrganizationUserCommand, Result<LoginOrganizationUserResult>>
{
    private readonly IAuthService _authService;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<LoginOrganizationUserCommandHandler> _logger;

    public LoginOrganizationUserCommandHandler(
        IAuthService authService,
        IOrganizationUserRepository organizationUserRepository,
        IOrganizationRepository organizationRepository,
        ILogger<LoginOrganizationUserCommandHandler> logger)
    {
        _authService = authService;
        _organizationUserRepository = organizationUserRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<LoginOrganizationUserResult>> Handle(
        LoginOrganizationUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Organization user login attempt for email: {Email}", request.Email);

        // 1. Authenticate with Supabase
        var authResult = await _authService.SignInAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        if (authResult.User is null)
        {
            _logger.LogWarning("Login failed for organization user: {Email}", request.Email);
            return Result.Failure<LoginOrganizationUserResult>("Invalid email or password");
        }

        // 2. Get OrganizationUser from domain by SupabaseUserId
        if (!Guid.TryParse(authResult.User.Id, out var supabaseUserId))
        {
            _logger.LogError("Invalid Supabase user ID format: {UserId}", authResult.User.Id);
            return Result.Failure<LoginOrganizationUserResult>("Authentication error");
        }

        var organizationUser = await _organizationUserRepository.GetBySupabaseUserIdAsync(
            supabaseUserId,
            cancellationToken
        );

        if (organizationUser is null)
        {
            _logger.LogWarning(
                "Organization user not found for Supabase ID: {SupabaseUserId}",
                supabaseUserId
            );
            return Result.Failure<LoginOrganizationUserResult>("User account not found");
        }

        // 3. Validate account is activated
        if (!organizationUser.IsActivated)
        {
            _logger.LogWarning(
                "Login attempt with inactive account: {Email}",
                request.Email
            );
            return Result.Failure<LoginOrganizationUserResult>(
                "Account not activated. Please activate your account first."
            );
        }

        // 4. Get organization details
        var organization = await _organizationRepository.GetByIdAsync(
            organizationUser.OrganizationId,
            cancellationToken
        );

        if (organization is null)
        {
            _logger.LogError(
                "Organization not found for user: {UserId}, OrgId: {OrganizationId}",
                organizationUser.Id,
                organizationUser.OrganizationId
            );
            return Result.Failure<LoginOrganizationUserResult>("Organization not found");
        }

        _logger.LogInformation(
            "Organization user logged in successfully: {Email}, Organization: {OrganizationName}",
            request.Email,
            organization.Name
        );

        // 5. Return successful login result
        return Result.Success(new LoginOrganizationUserResult
        {
            AccessToken = authResult.AccessToken ?? string.Empty,
            RefreshToken = authResult.RefreshToken ?? string.Empty,
            ExpiresAt = authResult.ExpiresAt ?? DateTimeOffset.UtcNow.AddHours(1),
            UserId = organizationUser.Id,
            OrganizationId = organizationUser.OrganizationId,
            OrganizationName = organization.Name,
            Email = organizationUser.Email.Value,
            FullName = $"{organizationUser.FirstName} {organizationUser.LastName}",
            Role = organizationUser.Role.ToString(),
            Status = "Active", // TODO: Add status to OrganizationUser entity if needed
            IsActivated = organizationUser.IsActivated
        });
    }
}
