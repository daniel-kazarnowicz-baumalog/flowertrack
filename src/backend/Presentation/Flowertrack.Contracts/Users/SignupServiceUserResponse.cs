namespace Flowertrack.Contracts.Users;

/// <summary>
/// Response after creating a new service user
/// </summary>
public sealed record SignupServiceUserResponse
{
    /// <summary>
    /// ID of the created service user
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Full name
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// Activation token (if password was not provided)
    /// Used to set initial password via activation link
    /// </summary>
    public string? ActivationToken { get; init; }

    /// <summary>
    /// Token expiry date
    /// </summary>
    public DateTimeOffset? ActivationTokenExpiresAt { get; init; }
}
