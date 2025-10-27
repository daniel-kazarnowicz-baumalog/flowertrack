namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an invitation email to a new organization administrator
    /// </summary>
    Task SendOrganizationInvitationAsync(
        string email,
        string firstName,
        string organizationName,
        string activationLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an invitation email to a new service user
    /// </summary>
    Task SendServiceUserInvitationAsync(
        string email,
        string firstName,
        string activationLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an invitation email to a new organization user
    /// </summary>
    Task SendOrganizationUserInvitationAsync(
        string email,
        string firstName,
        string organizationName,
        string activationLink,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default);
}
