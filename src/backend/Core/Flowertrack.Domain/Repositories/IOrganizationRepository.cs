using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Organization aggregate root.
/// Provides specialized query methods for organization management.
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// Gets an organization by its name.
    /// </summary>
    /// <param name="name">The organization name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The organization if found; otherwise, null.</returns>
    Task<Organization?> GetByNameAsync(string name, CancellationToken ct = default);

    /// <summary>
    /// Gets an organization by its API key.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The organization if found; otherwise, null.</returns>
    Task<Organization?> GetByApiKeyAsync(string apiKey, CancellationToken ct = default);

    /// <summary>
    /// Checks if an organization name already exists.
    /// </summary>
    /// <param name="name">The organization name to check.</param>
    /// <param name="excludeId">Optional organization ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the name exists; otherwise, false.</returns>
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);

    /// <summary>
    /// Checks if an API key already exists.
    /// </summary>
    /// <param name="apiKey">The API key to check.</param>
    /// <param name="excludeId">Optional organization ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the API key exists; otherwise, false.</returns>
    Task<bool> ApiKeyExistsAsync(string apiKey, Guid? excludeId = null, CancellationToken ct = default);

    /// <summary>
    /// Gets all active organizations (with valid contracts).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of active organizations.</returns>
    Task<IReadOnlyList<Organization>> GetActiveOrganizationsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets all organizations with expired contracts.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of organizations with expired contracts.</returns>
    Task<IReadOnlyList<Organization>> GetExpiredContractsAsync(CancellationToken ct = default);
}
