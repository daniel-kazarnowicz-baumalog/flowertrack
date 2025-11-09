namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to send password reset email
/// </summary>
public sealed record ForgotPasswordRequest
{
    /// <summary>
    /// Email address of the user requesting password reset
    /// </summary>
    public required string Email { get; init; }
}
