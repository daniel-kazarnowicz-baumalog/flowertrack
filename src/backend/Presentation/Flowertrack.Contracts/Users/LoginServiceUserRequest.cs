namespace Flowertrack.Contracts.Users;

/// <summary>
/// Request to login a service user
/// </summary>
public sealed record LoginServiceUserRequest
{
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
