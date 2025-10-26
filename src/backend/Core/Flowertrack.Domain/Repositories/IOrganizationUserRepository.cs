using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for OrganizationUser entity.
/// Provides specialized query methods for organization user management.
/// </summary>
public interface IOrganizationUserRepository : IRepository<OrganizationUser>
{
    /// <summary>
    /// Gets an organization user by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The organization user if found; otherwise, null.</returns>
    Task<OrganizationUser?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Gets all users for a specific organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of organization users.</returns>
    Task<IReadOnlyList<OrganizationUser>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default);

    /// <summary>
    /// Checks if an email address already exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeId">Optional user ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the email exists; otherwise, false.</returns>
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
}
