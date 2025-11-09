namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to activate organization user account
/// </summary>
public sealed record ActivateAccountRequest
{
    /// <summary>
    /// Activation token from email or user setup
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Optional: Set initial password during activation
    /// If not provided, user must have already confirmed email with Supabase
    /// </summary>
    public string? Password { get; init; }
}
