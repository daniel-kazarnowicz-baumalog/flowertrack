using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.SignupServiceUser;

/// <summary>
/// Handler for SignupServiceUserCommand
/// Creates service user in Supabase Auth and domain
/// </summary>
public sealed class SignupServiceUserCommandHandler
    : IRequestHandler<SignupServiceUserCommand, Result<SignupServiceUserResult>>
{
    private readonly IAuthService _authService;
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ILogger<SignupServiceUserCommandHandler> _logger;

    public SignupServiceUserCommandHandler(
        IAuthService authService,
        IServiceUserRepository serviceUserRepository,
        ILogger<SignupServiceUserCommandHandler> logger)
    {
        _authService = authService;
        _serviceUserRepository = serviceUserRepository;
        _logger = logger;
    }

    public async Task<Result<SignupServiceUserResult>> Handle(
        SignupServiceUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating service user account for email: {Email}",
            request.Email
        );

        // 1. Check if email already exists in domain
        var existingUser = await _serviceUserRepository.GetByEmailAsync(
            request.Email,
            cancellationToken
        );

        if (existingUser is not null)
        {
            _logger.LogWarning(
                "Service user with email {Email} already exists",
                request.Email
            );
            return Result.Failure<SignupServiceUserResult>(
                "A service user with this email already exists"
            );
        }

        // 2. Create user in Supabase Auth
        var metadata = new UserMetadata
        {
            FullName = $"{request.FirstName} {request.LastName}",
            Role = "service_user"
        };

        var authResult = await _authService.SignUpAsync(
            request.Email,
            request.Password ?? string.Empty, // Empty password triggers Supabase email verification
            metadata,
            cancellationToken
        );

        if (authResult.User is null)
        {
            _logger.LogError(
                "Failed to create Supabase account for {Email}: {Error}",
                request.Email,
                authResult.ErrorMessage
            );
            return Result.Failure<SignupServiceUserResult>(
                authResult.ErrorMessage ?? "Failed to create user account"
            );
        }

        // 3. Create ServiceUser domain entity
        var serviceUser = ServiceUser.Create(
            userId: Guid.NewGuid(), // Generate domain ID
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            specialization: request.Specialization
        );

        // Link to Supabase user
        if (Guid.TryParse(authResult.User.Id, out var supabaseUserId))
        {
            serviceUser.LinkToSupabaseUser(supabaseUserId);
        }

        await _serviceUserRepository.AddAsync(serviceUser, cancellationToken);

        _logger.LogInformation(
            "Service user created successfully. UserId: {UserId}, Email: {Email}",
            serviceUser.Id,
            serviceUser.Email
        );

        return Result.Success(new SignupServiceUserResult
        {
            UserId = serviceUser.Id,
            Email = serviceUser.Email,
            FullName = $"{serviceUser.FirstName} {serviceUser.LastName}",
            ActivationToken = null, // Supabase handles activation via email
            ActivationTokenExpiresAt = null
        });
    }
}
