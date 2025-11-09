using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Entities.Authentication;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public sealed class RefreshToken : Entity<Guid>
{
    /// <summary>
    /// User identifier this token belongs to
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The refresh token value
    /// </summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// Expiration date and time
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// Creation date and time
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Revocation date and time (null if not revoked)
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    /// IP address from which the token was created
    /// </summary>
    public string? CreatedByIp { get; private set; }

    /// <summary>
    /// IP address from which the token was revoked
    /// </summary>
    public string? RevokedByIp { get; private set; }

    /// <summary>
    /// Token that replaced this token (for token rotation)
    /// </summary>
    public string? ReplacedByToken { get; private set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string? RevocationReason { get; private set; }

    /// <summary>
    /// Indicates if the token is active (not expired and not revoked)
    /// </summary>
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates if the token is expired
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private RefreshToken() : base(Guid.Empty)
    {
    }

    /// <summary>
    /// Creates a new refresh token
    /// </summary>
    public static RefreshToken Create(
        Guid userId,
        string token,
        int expirationDays,
        string? createdByIp = null)
    {
        var id = Guid.NewGuid();
        return new RefreshToken(id)
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByIp = createdByIp
        };
    }

    /// <summary>
    /// Private constructor with ID for entity creation
    /// </summary>
    private RefreshToken(Guid id) : base(id)
    {
    }

    /// <summary>
    /// Revokes the refresh token
    /// </summary>
    public void Revoke(string? revokedByIp = null, string? reason = null, string? replacedByToken = null)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        RevokedByIp = revokedByIp;
        RevocationReason = reason;
        ReplacedByToken = replacedByToken;
    }
}
