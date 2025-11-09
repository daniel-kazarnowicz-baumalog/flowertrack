using Flowertrack.Domain.Entities.Users;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Role entity
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role by ID
    /// </summary>
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles
    /// </summary>
    Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role exists by ID
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
