using Flowertrack.Domain.Entities.Users;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for OrganizationUser aggregate.
/// </summary>
public interface IOrganizationUserRepository : IRepository<OrganizationUser>
{
    /// <summary>
    /// Gets an organization user by their user ID (from identity system).
    /// </summary>
    /// <param name="userId">The user identifier from the identity system.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The organization user if found; otherwise, null.</returns>
    Task<OrganizationUser?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all users for a specific organization.
    /// </summary>
    /// <param name="orgId">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of users for the organization.</returns>
    Task<List<OrganizationUser>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active users for a specific organization.
    /// </summary>
    /// <param name="orgId">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of active users for the organization.</returns>
    Task<List<OrganizationUser>> GetActiveByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if an organization user with the given email already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if an organization user with the email exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
