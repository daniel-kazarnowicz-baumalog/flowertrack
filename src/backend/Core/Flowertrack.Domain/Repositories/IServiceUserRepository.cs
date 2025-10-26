using Flowertrack.Domain.Entities;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for ServiceUser aggregate.
/// </summary>
public interface IServiceUserRepository : IRepository<ServiceUser>
{
    /// <summary>
    /// Gets a service user by their user ID (from identity system).
    /// </summary>
    Task<ServiceUser?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active service users.
    /// </summary>
    Task<List<ServiceUser>> GetActiveServiceUsersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a service user with the given email already exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
