using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.InviteOrganizationUser;

/// <summary>
/// Handler for InviteOrganizationUserCommand
/// Creates organization user in Supabase Auth and domain, sends invitation email
/// </summary>
public sealed class InviteOrganizationUserCommandHandler
    : IRequestHandler<InviteOrganizationUserCommand, Result<InviteOrganizationUserResult>>
{
    private readonly IAuthService _authService;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<InviteOrganizationUserCommandHandler> _logger;

    public InviteOrganizationUserCommandHandler(
        IAuthService authService,
        IOrganizationUserRepository organizationUserRepository,
        IOrganizationRepository organizationRepository,
        ILogger<InviteOrganizationUserCommandHandler> logger)
    {
        _authService = authService;
        _organizationUserRepository = organizationUserRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<InviteOrganizationUserResult>> Handle(
        InviteOrganizationUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Inviting organization user: {Email} to organization: {OrganizationId}",
            request.Email,
            request.OrganizationId
        );

        // 1. Verify organization exists
        var organization = await _organizationRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken
        );

        if (organization is null)
        {
            return Result.Failure<InviteOrganizationUserResult>("Organization not found");
        }

        // 2. Check if email already exists in this organization
        var existingUser = await _organizationUserRepository.GetByEmailAsync(
            request.Email,
            cancellationToken
        );

        if (existingUser is not null && existingUser.OrganizationId == request.OrganizationId)
        {
            _logger.LogWarning(
                "User with email {Email} already exists in organization {OrganizationId}",
                request.Email,
                request.OrganizationId
            );
            return Result.Failure<InviteOrganizationUserResult>(
                "A user with this email already exists in the organization"
            );
        }

        // 3. Validate role
        var roleString = request.Role.ToLowerInvariant() switch
        {
            "organization_admin" or "admin" => Flowertrack.Domain.Enums.OrganizationUserRole.Admin,
            "organization_operator" or "user" => Flowertrack.Domain.Enums.OrganizationUserRole.User,
            "owner" => Flowertrack.Domain.Enums.OrganizationUserRole.Owner,
            _ => null
        };

        if (roleString is null || !Flowertrack.Domain.Enums.OrganizationUserRole.IsValid(roleString))
        {
            return Result.Failure<InviteOrganizationUserResult>(
                $"Invalid role: {request.Role}. Must be Admin, User, or Owner"
            );
        }

        // 4. Create user in Supabase Auth
        var metadata = new UserMetadata
        {
            FullName = $"{request.FirstName} {request.LastName}",
            Role = roleString,
            OrganizationId = request.OrganizationId
        };

        var authResult = await _authService.SignUpAsync(
            request.Email,
            string.Empty, // Empty password - user will set via email confirmation
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
            return Result.Failure<InviteOrganizationUserResult>(
                authResult.ErrorMessage ?? "Failed to create user account"
            );
        }

        // 5. Create OrganizationUser domain entity
        var organizationUser = OrganizationUser.Create(
            userId: Guid.NewGuid(), // Generate new domain user ID
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            organizationId: request.OrganizationId,
            role: roleString,
            phoneNumber: request.PhoneNumber
        );

        // Link to Supabase user
        if (Guid.TryParse(authResult.User.Id, out var supabaseUserId))
        {
            organizationUser.LinkToSupabaseUser(supabaseUserId);
        }

        // Generate invitation token (7 days validity)
        var invitationToken = Guid.NewGuid().ToString("N");
        var tokenExpiry = DateTimeOffset.UtcNow.AddDays(7);
        organizationUser.SetInvitationToken(invitationToken, tokenExpiry);

        await _organizationUserRepository.AddAsync(organizationUser, cancellationToken);

        _logger.LogInformation(
            "Organization user invited successfully. UserId: {UserId}, Email: {Email}, Organization: {OrganizationId}",
            organizationUser.Id,
            organizationUser.Email.Value,
            request.OrganizationId
        );

        return Result.Success(new InviteOrganizationUserResult
        {
            UserId = organizationUser.Id,
            Email = organizationUser.Email.Value,
            FullName = $"{organizationUser.FirstName} {organizationUser.LastName}",
            InvitationToken = invitationToken,
            InvitationTokenExpiresAt = tokenExpiry
        });
    }
}
