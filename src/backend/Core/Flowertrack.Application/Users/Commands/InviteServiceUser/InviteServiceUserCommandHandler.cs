using Flowertrack.Application.Common.Exceptions;
using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.InviteServiceUser;

/// <summary>
/// Handler for InviteServiceUserCommand
/// Creates service user in Supabase Auth and domain, sends invitation email
/// </summary>
public sealed class InviteServiceUserCommandHandler
    : IRequestHandler<InviteServiceUserCommand, Result<Guid>>
{
    private readonly IServiceUserRepository _serviceUserRepository;
    private readonly ISupabaseClient _supabaseClient;
    private readonly IEmailService _emailService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<InviteServiceUserCommandHandler> _logger;

    public InviteServiceUserCommandHandler(
        IServiceUserRepository serviceUserRepository,
        ISupabaseClient supabaseClient,
        IEmailService emailService,
        ITokenGenerator tokenGenerator,
        ILogger<InviteServiceUserCommandHandler> logger)
    {
        _serviceUserRepository = serviceUserRepository;
        _supabaseClient = supabaseClient;
        _emailService = emailService;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        InviteServiceUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check if email already exists
        if (await _serviceUserRepository.EmailExistsAsync(request.Email, null, cancellationToken))
        {
            throw new ConflictException("Email already exists in the system");
        }

        var existingUser = await _supabaseClient.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ConflictException("Email already exists in authentication system");
        }

        // 2. Create user in Supabase Auth
        var userId = await _supabaseClient.CreateUserAsync(
            email: request.Email,
            password: null, // Will be set during activation
            metadata: new
            {
                first_name = request.FirstName,
                last_name = request.LastName,
                user_type = "service"
            },
            emailConfirm: false,
            cancellationToken: cancellationToken
        );

        // 3. Create ServiceUser profile (Domain)
        var serviceUser = ServiceUser.Create(
            userId: userId,
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            specialization: request.Specialization
        );

        await _serviceUserRepository.AddAsync(serviceUser, cancellationToken);

        // 4. Generate activation token (valid for 7 days)
        var activationToken = _tokenGenerator.GenerateSecureToken();
        var tokenExpiry = DateTimeOffset.UtcNow.AddDays(7);

        await _supabaseClient.StoreActivationTokenAsync(
            userId,
            activationToken,
            tokenExpiry,
            cancellationToken
        );

        // 5. Send invitation email (fire-and-forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendServiceUserInvitationAsync(
                    email: request.Email,
                    firstName: request.FirstName,
                    activationLink: $"https://service.flowertrack.com/activate?token={activationToken}",
                    cancellationToken: CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invitation email to {Email}", request.Email);
            }
        }, cancellationToken);

        _logger.LogInformation(
            "Service user {UserId} invited with email {Email}",
            userId,
            request.Email
        );

        return Result.Success(userId);
    }
}
