using Flowertrack.Domain.Entities.Authentication;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for RefreshToken entity
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Gets a refresh token by its token value
    /// </summary>
    /// <param name="token">Token value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RefreshToken if found, null otherwise</returns>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active refresh tokens</returns>
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all active refresh tokens for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="reason">Reason for revocation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes expired tokens older than the specified date
    /// </summary>
    /// <param name="olderThan">Date threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveExpiredTokensAsync(DateTimeOffset olderThan, CancellationToken cancellationToken = default);
}
