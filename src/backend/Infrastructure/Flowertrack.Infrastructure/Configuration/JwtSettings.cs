namespace Flowertrack.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for JWT token generation and validation
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key used for signing JWT tokens (minimum 32 characters)
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// Token issuer identifier
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Token audience identifier
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Access token expiration time in minutes (default: 60)
    /// </summary>
    public int ExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// Refresh token expiration time in days (default: 7)
    /// </summary>
    public int RefreshTokenExpirationDays { get; init; } = 7;
}
