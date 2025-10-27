using Flowertrack.Application.Common.Exceptions;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Organizations.Commands.OnboardOrganization;

/// <summary>
/// Handler for OnboardOrganizationCommand
/// Creates organization, admin user, generates API key, and sends invitation email
/// </summary>
public sealed class OnboardOrganizationCommandHandler
    : IRequestHandler<OnboardOrganizationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationUserRepository _userRepository;
    private readonly ISupabaseClient _supabaseClient;
    private readonly IEmailService _emailService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<OnboardOrganizationCommandHandler> _logger;

    public OnboardOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IOrganizationUserRepository userRepository,
        ISupabaseClient supabaseClient,
        IEmailService emailService,
        ITokenGenerator tokenGenerator,
        ILogger<OnboardOrganizationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _supabaseClient = supabaseClient;
        _emailService = emailService;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        OnboardOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check if organization with same name already exists
        if (await _organizationRepository.NameExistsAsync(request.Name, null, cancellationToken))
        {
            return Result.Failure<Guid>("Organization with this name already exists");
        }

        // 2. Check if email already exists in system
        var existingUser = await _supabaseClient.GetUserByEmailAsync(request.AdminEmail, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure<Guid>("Email already exists in the system");
        }

        // 3. Create organization (Domain)
        var organization = Organization.Create(
            name: request.Name,
            email: request.AdminEmail,
            phone: request.Phone,
            address: request.Address,
            city: request.City,
            postalCode: request.PostalCode,
            country: request.Country,
            notes: request.Notes
        );

        // 4. Generate API Key for organization
        var apiKey = organization.GenerateApiKey();

        // 5. Add organization to repository
        await _organizationRepository.AddAsync(organization, cancellationToken);

        // 6. Create user in Supabase Auth (without password - will be set during activation)
        var userId = await _supabaseClient.CreateUserAsync(
            email: request.AdminEmail,
            password: null,
            metadata: new
            {
                first_name = request.AdminFirstName,
                last_name = request.AdminLastName,
                organization_id = organization.Id
            },
            emailConfirm: false,
            cancellationToken: cancellationToken
        );

        // 7. Create OrganizationUser profile (Domain)
        var adminUser = OrganizationUser.Create(
            userId: userId,
            firstName: request.AdminFirstName,
            lastName: request.AdminLastName,
            email: request.AdminEmail,
            organizationId: organization.Id,
            role: OrganizationUserRole.Admin
        );

        await _userRepository.AddAsync(adminUser, cancellationToken);

        // 8. Generate activation token (valid for 7 days)
        var activationToken = _tokenGenerator.GenerateSecureToken();
        var tokenExpiry = DateTimeOffset.UtcNow.AddDays(7);

        await _supabaseClient.StoreActivationTokenAsync(
            userId,
            activationToken,
            tokenExpiry,
            cancellationToken
        );

        // Note: SaveChanges will be called by UnitOfWorkBehavior

        // 9. Send invitation email (fire-and-forget, don't block response)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendOrganizationInvitationAsync(
                    email: request.AdminEmail,
                    firstName: request.AdminFirstName,
                    organizationName: organization.Name,
                    activationLink: $"https://app.flowertrack.com/activate?token={activationToken}",
                    cancellationToken: CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invitation email to {Email}", request.AdminEmail);
            }
        }, cancellationToken);

        _logger.LogInformation(
            "Organization {OrganizationId} onboarded with admin user {UserId}",
            organization.Id,
            userId
        );

        return Result.Success(organization.Id);
    }
}
