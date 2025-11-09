using Flowertrack.Domain.Entities.Users;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for UserRole entity
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole>
{
    /// <summary>
    /// Gets all user roles for a specific user
    /// </summary>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user role assignment
    /// </summary>
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<bool> UserHasRoleAsync(Guid userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all roles for a user
    /// </summary>
    Task RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
