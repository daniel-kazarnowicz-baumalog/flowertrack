using Flowertrack.Domain.Entities.Users;
using Flowertrack.Domain.Repositories;
using Flowertrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for UserRole entity
/// </summary>
public sealed class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserRole?> GetByUserAndRoleAsync(Guid userId, int roleId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UserRole>()
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> UserHasRoleAsync(Guid userId, int roleId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UserRole>()
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await Context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        Context.Set<UserRole>().RemoveRange(userRoles);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
