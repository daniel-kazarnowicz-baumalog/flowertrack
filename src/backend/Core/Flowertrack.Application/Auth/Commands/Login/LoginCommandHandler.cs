using Flowertrack.Application.Auth.DTOs;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities.Authentication;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Auth.Commands.Login;

/// <summary>
/// Handler for LoginCommand
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginCommandHandler> _logger;
    private const int RefreshTokenExpirationDays = 7;
    private const int AccessTokenExpirationSeconds = 3600; // 60 minutes

    public LoginCommandHandler(
        IServiceUserRepository serviceUserRepository,
        IOrganizationUserRepository organizationUserRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
    {
        _serviceUserRepository = serviceUserRepository;
        _organizationUserRepository = organizationUserRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Try to find ServiceUser first
        var serviceUser = await _serviceUserRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (serviceUser != null)
        {
            // Verify ServiceUser password
            if (serviceUser.PasswordHash == null || !_passwordHasher.VerifyPassword(request.Password, serviceUser.PasswordHash))
            {
                _logger.LogWarning("Invalid password for ServiceUser: {Email}", request.Email);
                return Result.Failure<LoginResponse>("Invalid email or password");
            }

            // Check if user is active
            if (serviceUser.Status != Domain.Enums.UserStatus.Active)
            {
                _logger.LogWarning("Inactive ServiceUser login attempt: {Email}, Status: {Status}", 
                    request.Email, serviceUser.Status);
                return Result.Failure<LoginResponse>("User account is not active");
            }

            // Get user roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(serviceUser.Id, cancellationToken);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateToken(serviceUser.Id, serviceUser.Email.Value, roleNames);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            // Create and store refresh token
            var refreshToken = Domain.Entities.Authentication.RefreshToken.Create(
                serviceUser.Id,
                refreshTokenValue,
                RefreshTokenExpirationDays,
                request.IpAddress
            );

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ServiceUser logged in successfully: {Email}", request.Email);

            var userDto = new UserDto(
                serviceUser.Id,
                serviceUser.Email.Value,
                serviceUser.FirstName,
                serviceUser.LastName,
                "ServiceUser",
                roleNames
            );

            return Result.Success(new LoginResponse(
                accessToken,
                refreshTokenValue,
                AccessTokenExpirationSeconds,
                userDto
            ));
        }

        // Try to find OrganizationUser
        var organizationUser = await _organizationUserRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (organizationUser != null)
        {
            // Verify OrganizationUser password
            if (organizationUser.PasswordHash == null || !_passwordHasher.VerifyPassword(request.Password, organizationUser.PasswordHash))
            {
                _logger.LogWarning("Invalid password for OrganizationUser: {Email}", request.Email);
                return Result.Failure<LoginResponse>("Invalid email or password");
            }

            // Check if user is active
            if (organizationUser.Status != Domain.Enums.UserStatus.Active)
            {
                _logger.LogWarning("Inactive OrganizationUser login attempt: {Email}, Status: {Status}", 
                    request.Email, organizationUser.Status);
                return Result.Failure<LoginResponse>("User account is not active");
            }

            // Check if account is activated
            if (!organizationUser.IsActivated)
            {
                _logger.LogWarning("Unactivated OrganizationUser login attempt: {Email}", request.Email);
                return Result.Failure<LoginResponse>("Please activate your account first");
            }

            // Get user roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(organizationUser.Id, cancellationToken);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            // Generate tokens
            var accessToken = _jwtTokenGenerator.GenerateToken(organizationUser.Id, organizationUser.Email.Value, roleNames);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            // Create and store refresh token
            var refreshToken = Domain.Entities.Authentication.RefreshToken.Create(
                organizationUser.Id,
                refreshTokenValue,
                RefreshTokenExpirationDays,
                request.IpAddress
            );

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("OrganizationUser logged in successfully: {Email}", request.Email);

            var userDto = new UserDto(
                organizationUser.Id,
                organizationUser.Email.Value,
                organizationUser.FirstName,
                organizationUser.LastName,
                "OrganizationUser",
                roleNames,
                organizationUser.OrganizationId
            );

            return Result.Success(new LoginResponse(
                accessToken,
                refreshTokenValue,
                AccessTokenExpirationSeconds,
                userDto
            ));
        }

        // User not found
        _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
        return Result.Failure<LoginResponse>("Invalid email or password");
    }
}
