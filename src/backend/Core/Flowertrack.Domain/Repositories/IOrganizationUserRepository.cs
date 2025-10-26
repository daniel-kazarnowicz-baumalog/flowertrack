using Flowertrack.Domain.Entities;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for OrganizationUser aggregate.
/// </summary>
public interface IOrganizationUserRepository : IRepository<OrganizationUser>
{
    /// <summary>
    /// Gets an organization user by their user ID (from identity system).
    /// </summary>
    Task<OrganizationUser?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all users for a specific organization.
    /// </summary>
    Task<List<OrganizationUser>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active users for a specific organization.
    /// </summary>
    Task<List<OrganizationUser>> GetActiveByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if an organization user with the given email already exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
