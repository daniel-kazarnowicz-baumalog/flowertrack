namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to reset password with new password
/// </summary>
public sealed record ResetPasswordRequest
{
    /// <summary>
    /// New password for the account
    /// </summary>
    public required string NewPassword { get; init; }

    /// <summary>
    /// Password reset token (from email link)
    /// </summary>
    public required string Token { get; init; }
}
