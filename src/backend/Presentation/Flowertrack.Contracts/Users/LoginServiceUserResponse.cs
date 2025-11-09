namespace Flowertrack.Contracts.Users;

/// <summary>
/// Response after successful login
/// </summary>
public sealed record LoginServiceUserResponse
{
    /// <summary>
    /// Access token (JWT)
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Token expiration timestamp
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// User information
    /// </summary>
    public ServiceUserDto User { get; init; } = null!;
}

/// <summary>
/// Service user data transfer object
/// </summary>
public sealed record ServiceUserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
