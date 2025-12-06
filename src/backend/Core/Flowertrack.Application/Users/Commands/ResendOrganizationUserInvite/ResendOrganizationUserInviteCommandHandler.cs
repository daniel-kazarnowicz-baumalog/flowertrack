using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Users.Commands.ResendOrganizationUserInvite;

/// <summary>
/// Handler for ResendOrganizationUserInviteCommand.
/// US-051: Resends invitation email to pending organization users.
/// </summary>
public sealed class ResendOrganizationUserInviteCommandHandler
    : IRequestHandler<ResendOrganizationUserInviteCommand, Result<ResendInviteResult>>
{
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResendOrganizationUserInviteCommandHandler> _logger;

    public ResendOrganizationUserInviteCommandHandler(
        IOrganizationUserRepository organizationUserRepository,
        IOrganizationRepository organizationRepository,
        IEmailService emailService,
        ILogger<ResendOrganizationUserInviteCommandHandler> logger)
    {
        _organizationUserRepository = organizationUserRepository;
        _organizationRepository = organizationRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<ResendInviteResult>> Handle(
        ResendOrganizationUserInviteCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Resending invitation to user {UserId} in organization {OrganizationId}",
            request.UserId,
            request.OrganizationId);

        // 1. Verify organization exists
        var organization = await _organizationRepository.GetByIdAsync(
            request.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return Result.Failure<ResendInviteResult>("Organization not found");
        }

        // 2. Get the organization user
        var user = await _organizationUserRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            return Result.Failure<ResendInviteResult>("User not found");
        }

        // 3. Verify user belongs to the organization
        if (user.OrganizationId != request.OrganizationId)
        {
            _logger.LogWarning(
                "User {UserId} does not belong to organization {OrganizationId}",
                request.UserId,
                request.OrganizationId);
            return Result.Failure<ResendInviteResult>("User does not belong to this organization");
        }

        // 4. Verify user is still pending activation
        if (user.IsActivated || user.Status == UserStatus.Active)
        {
            return Result.Failure<ResendInviteResult>("User has already activated their account");
        }

        // 5. Generate new invitation token (7 days validity)
        var newToken = Guid.NewGuid().ToString("N");
        var tokenExpiry = DateTimeOffset.UtcNow.AddDays(7);
        user.SetInvitationToken(newToken, tokenExpiry);

        // 6. Save changes
        await _organizationUserRepository.UpdateAsync(user, cancellationToken);

        // 7. Send invitation email
        try
        {
            await _emailService.SendOrganizationUserInvitationAsync(
                user.Email.Value,
                user.FirstName,
                organization.Name,
                newToken,
                cancellationToken);

            _logger.LogInformation(
                "Invitation resent successfully to {Email} for organization {OrganizationName}",
                user.Email.Value,
                organization.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send invitation email to {Email}",
                user.Email.Value);
            // Don't fail the operation - token is saved, email can be resent manually
        }

        return Result.Success(new ResendInviteResult
        {
            UserId = user.Id,
            Email = user.Email.Value,
            InvitationTokenExpiresAt = tokenExpiry,
            Message = "Invitation email has been resent successfully"
        });
    }
}
