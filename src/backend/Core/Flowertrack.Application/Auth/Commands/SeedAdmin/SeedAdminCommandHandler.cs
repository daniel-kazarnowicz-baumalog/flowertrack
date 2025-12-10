using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Auth.Commands.SeedAdmin;

/// <summary>
/// Handler for SeedAdminCommand.
/// Creates the initial Service Administrator user in both Supabase and the application database.
/// </summary>
public sealed class SeedAdminCommandHandler : IRequestHandler<SeedAdminCommand, Result<SeedAdminResponse>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ISupabaseClient _supabaseClient;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SeedAdminCommandHandler> _logger;

    public SeedAdminCommandHandler(
        IServiceUserRepository serviceUserRepository,
        ISupabaseClient supabaseClient,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<SeedAdminCommandHandler> logger)
    {
        _serviceUserRepository = serviceUserRepository;
        _supabaseClient = supabaseClient;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SeedAdminResponse>> Handle(SeedAdminCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding admin user: {Email}", request.Email);

        try
        {
            // Check if admin already exists
            var existingUser = await _serviceUserRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogInformation("Admin user already exists: {Email}", request.Email);
                return Result.Success(new SeedAdminResponse(
                    existingUser.Id,
                    existingUser.SupabaseUserId,
                    request.Email,
                    existingUser.FullName,
                    "Admin user already exists"));
            }

            // Try to create user in Supabase
            Guid? supabaseUserId = null;
            try
            {
                var existingSupabaseUser = await _supabaseClient.GetUserByEmailAsync(request.Email, cancellationToken);
                if (existingSupabaseUser != null)
                {
                    supabaseUserId = Guid.Parse(existingSupabaseUser.Id!);
                    _logger.LogInformation("Supabase user already exists: {SupabaseUserId}", supabaseUserId);
                }
                else
                {
                    supabaseUserId = await _supabaseClient.CreateUserAsync(
                        request.Email,
                        request.Password,
                        new { role = "ServiceAdministrator", first_name = request.FirstName, last_name = request.LastName },
                        emailConfirm: true,
                        cancellationToken);
                    _logger.LogInformation("Created Supabase user: {SupabaseUserId}", supabaseUserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create/get Supabase user. Continuing with local user only.");
            }

            // Create ServiceUser in application database
            var serviceUser = ServiceUser.Create(
                Guid.NewGuid(),
                request.FirstName,
                request.LastName,
                request.Email,
                phoneNumber: null,
                specialization: "System Administration");

            // Link to Supabase if available
            if (supabaseUserId.HasValue)
            {
                serviceUser.LinkToSupabaseUser(supabaseUserId.Value);
            }

            // Set password hash for local authentication
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            serviceUser.SetPasswordHash(passwordHash);

            // Activate the user
            serviceUser.Activate();

            // Assign ServiceAdministrator role (id = 1)
            serviceUser.AssignRole(1);

            await _serviceUserRepository.AddAsync(serviceUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Admin user seeded successfully: {UserId}, Email: {Email}", 
                serviceUser.Id, request.Email);

            return Result.Success(new SeedAdminResponse(
                serviceUser.Id,
                supabaseUserId,
                request.Email,
                serviceUser.FullName,
                "Admin user created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed admin user: {Email}", request.Email);
            return Result.Failure<SeedAdminResponse>($"Failed to seed admin user: {ex.Message}");
        }
    }
}
