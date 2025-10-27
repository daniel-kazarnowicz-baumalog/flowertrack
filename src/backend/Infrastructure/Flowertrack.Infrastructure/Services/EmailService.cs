using Flowertrack.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Email service implementation using Supabase/SendGrid
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly ISupabaseClient _supabaseClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        ISupabaseClient supabaseClient,
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _supabaseClient = supabaseClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrganizationInvitationAsync(
        string email,
        string firstName,
        string organizationName,
        string activationLink,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Welcome to FLOWerTRACK - {organizationName}";
        var body = GetOrganizationInvitationEmailBody(firstName, organizationName, activationLink);

        await _supabaseClient.SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation(
            "Organization invitation email sent to {Email} for organization {OrganizationName}",
            email,
            organizationName
        );
    }

    public async Task SendServiceUserInvitationAsync(
        string email,
        string firstName,
        string activationLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to FLOWerTRACK Service Team";
        var body = GetServiceUserInvitationEmailBody(firstName, activationLink);

        await _supabaseClient.SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Service user invitation email sent to {Email}", email);
    }

    public async Task SendOrganizationUserInvitationAsync(
        string email,
        string firstName,
        string organizationName,
        string activationLink,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Invitation to join {organizationName} on FLOWerTRACK";
        var body = GetOrganizationUserInvitationEmailBody(firstName, organizationName, activationLink);

        await _supabaseClient.SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation(
            "Organization user invitation email sent to {Email} for organization {OrganizationName}",
            email,
            organizationName
        );
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var subject = "Password Reset Request - FLOWerTRACK";
        var body = GetPasswordResetEmailBody(resetLink);

        await _supabaseClient.SendEmailAsync(email, subject, body, cancellationToken);

        _logger.LogInformation("Password reset email sent to {Email}", email);
    }

    private static string GetOrganizationInvitationEmailBody(
        string firstName,
        string organizationName,
        string activationLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Welcome to FLOWerTRACK</title>
</head>
<body style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <h2>Welcome to FLOWerTRACK, {firstName}!</h2>

    <p>Your organization <strong>{organizationName}</strong> has been registered on FLOWerTRACK.</p>

    <p>To activate your administrator account and set your password, please click the link below:</p>

    <p style=""margin: 30px 0;"">
        <a href=""{activationLink}"" style=""background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;"">
            Activate Account
        </a>
    </p>

    <p><strong>Important:</strong> This activation link is valid for 7 days.</p>

    <p>If you have any questions, please contact our support team.</p>

    <p>Best regards,<br>The FLOWerTRACK Team</p>

    <hr style=""margin-top: 40px; border: none; border-top: 1px solid #ddd;"">
    <p style=""font-size: 12px; color: #666;"">
        If you didn't expect this email, please ignore it or contact support.
    </p>
</body>
</html>";
    }

    private static string GetServiceUserInvitationEmailBody(
        string firstName,
        string activationLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Welcome to FLOWerTRACK Service Team</title>
</head>
<body style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <h2>Welcome to the Service Team, {firstName}!</h2>

    <p>You have been invited to join the FLOWerTRACK service team.</p>

    <p>To activate your account and set your password, please click the link below:</p>

    <p style=""margin: 30px 0;"">
        <a href=""{activationLink}"" style=""background-color: #2196F3; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;"">
            Activate Account
        </a>
    </p>

    <p><strong>Important:</strong> This activation link is valid for 7 days.</p>

    <p>Best regards,<br>The FLOWerTRACK Team</p>

    <hr style=""margin-top: 40px; border: none; border-top: 1px solid #ddd;"">
    <p style=""font-size: 12px; color: #666;"">
        If you didn't expect this email, please contact your administrator.
    </p>
</body>
</html>";
    }

    private static string GetOrganizationUserInvitationEmailBody(
        string firstName,
        string organizationName,
        string activationLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Invitation to {organizationName}</title>
</head>
<body style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <h2>Hello {firstName}!</h2>

    <p>You have been invited to join <strong>{organizationName}</strong> on FLOWerTRACK.</p>

    <p>To activate your account and set your password, please click the link below:</p>

    <p style=""margin: 30px 0;"">
        <a href=""{activationLink}"" style=""background-color: #4CAF50; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;"">
            Activate Account
        </a>
    </p>

    <p><strong>Important:</strong> This activation link is valid for 7 days.</p>

    <p>Best regards,<br>{organizationName}</p>

    <hr style=""margin-top: 40px; border: none; border-top: 1px solid #ddd;"">
    <p style=""font-size: 12px; color: #666;"">
        If you didn't expect this email, please contact your organization administrator.
    </p>
</body>
</html>";
    }

    private static string GetPasswordResetEmailBody(string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Password Reset</title>
</head>
<body style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <h2>Password Reset Request</h2>

    <p>We received a request to reset your password for your FLOWerTRACK account.</p>

    <p>To reset your password, please click the link below:</p>

    <p style=""margin: 30px 0;"">
        <a href=""{resetLink}"" style=""background-color: #FF9800; color: white; padding: 14px 20px; text-decoration: none; display: inline-block; border-radius: 4px;"">
            Reset Password
        </a>
    </p>

    <p><strong>Important:</strong> This reset link is valid for 24 hours.</p>

    <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>

    <p>Best regards,<br>The FLOWerTRACK Team</p>
</body>
</html>";
    }
}
