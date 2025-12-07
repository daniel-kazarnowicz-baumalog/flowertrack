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
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<OnboardOrganizationCommandHandler> _logger;

    public OnboardOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IOrganizationUserRepository userRepository,
        ISupabaseClient supabaseClient,
        IEmailService emailService,
        ITokenGenerator tokenGenerator,
        IPasswordHasher passwordHasher,
        ILogger<OnboardOrganizationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _supabaseClient = supabaseClient;
        _emailService = emailService;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        OnboardOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check if organization with same name already exists (ignore soft-deleted)
        if (await _organizationRepository.NameExistsAsync(request.Name, null, cancellationToken))
        {
            return Result.Failure<Guid>("Organization with this name already exists");
        }

        // 2. Check if email already exists in local database (ignore soft-deleted users)
        var existingLocalUser = await _userRepository.GetByEmailAsync(request.AdminEmail, cancellationToken);
        if (existingLocalUser != null && !existingLocalUser.IsDeleted)
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

        // 6. Create user in Supabase Auth WITH password "Password123!"
        _logger.LogInformation("Creating Supabase user for email: {Email}", request.AdminEmail);
        
        var supabaseUserId = await _supabaseClient.CreateUserAsync(
            email: request.AdminEmail,
            password: "Password123!", // Set explicit password for development
            metadata: new
            {
                first_name = request.AdminFirstName,
                last_name = request.AdminLastName,
                organization_id = organization.Id
            },
            emailConfirm: true, // Auto-confirm email for development
            cancellationToken: cancellationToken
        );

        _logger.LogInformation("Supabase user created with ID: {SupabaseUserId}", supabaseUserId);

        // 7. Create OrganizationUser profile (Domain) - use Supabase user ID
        var adminUser = OrganizationUser.Create(
            userId: supabaseUserId, // Use Supabase user ID as primary ID
            firstName: request.AdminFirstName,
            lastName: request.AdminLastName,
            email: request.AdminEmail,
            organizationId: organization.Id,
            role: OrganizationUserRole.Admin
        );

        // 7a. Set password hash (same password as Supabase) and activate user
        _logger.LogInformation("Setting password hash and activating user");
        var defaultPasswordHash = _passwordHasher.HashPassword("Password123!");
        adminUser.SetPasswordHash(defaultPasswordHash);
        adminUser.LinkToSupabaseUser(supabaseUserId); // Link to Supabase
        adminUser.Activate(); // Activate user immediately
        
        await _userRepository.AddAsync(adminUser, cancellationToken);

        _logger.LogInformation("Organization {OrgId} onboarded with user {UserId}", 
            organization.Id, supabaseUserId);

        // Note: SaveChanges will be called by UnitOfWorkBehavior

        return Result.Success(organization.Id);
    }
}
