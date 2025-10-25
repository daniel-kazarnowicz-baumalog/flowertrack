using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for ServiceUser entity.
/// Provides specialized query methods for service user management.
/// </summary>
public interface IServiceUserRepository : IRepository<ServiceUser>
{
    /// <summary>
    /// Gets a service user by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The service user if found; otherwise, null.</returns>
    Task<ServiceUser?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Gets all active service users.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of active service users.</returns>
    Task<IReadOnlyList<ServiceUser>> GetActiveUsersAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets all available service users (active and available for assignment).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of available service users.</returns>
    Task<IReadOnlyList<ServiceUser>> GetAvailableUsersAsync(CancellationToken ct = default);

    /// <summary>
    /// Checks if an email address already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeId">Optional user ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the email exists; otherwise, false.</returns>
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
}
