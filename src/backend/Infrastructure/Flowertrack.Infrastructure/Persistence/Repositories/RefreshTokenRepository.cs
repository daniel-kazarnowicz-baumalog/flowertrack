using Flowertrack.Domain.Entities.Authentication;
using Flowertrack.Domain.Repositories;
using Flowertrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for RefreshToken entity
/// </summary>
public sealed class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RevokeAllUserTokensAsync(
        Guid userId, 
        string reason, 
        CancellationToken cancellationToken = default)
    {
        var activeTokens = await Context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke(reason: reason);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveExpiredTokensAsync(
        DateTimeOffset olderThan, 
        CancellationToken cancellationToken = default)
    {
        var expiredTokens = await Context.Set<RefreshToken>()
            .Where(rt => rt.ExpiresAt < olderThan)
            .ToListAsync(cancellationToken);

        Context.Set<RefreshToken>().RemoveRange(expiredTokens);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
