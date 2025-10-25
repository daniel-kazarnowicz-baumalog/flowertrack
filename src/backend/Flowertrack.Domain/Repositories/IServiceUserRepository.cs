using Flowertrack.Domain.Entities.Users;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for ServiceUser aggregate.
/// </summary>
public interface IServiceUserRepository : IRepository<ServiceUser>
{
    /// <summary>
    /// Gets a service user by their user ID (from identity system).
    /// </summary>
    /// <param name="userId">The user identifier from the identity system.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The service user if found; otherwise, null.</returns>
    Task<ServiceUser?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active service users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all active service users.</returns>
    Task<List<ServiceUser>> GetActiveServiceUsersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a service user with the given email already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a service user with the email exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
