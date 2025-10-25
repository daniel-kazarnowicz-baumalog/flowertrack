using Flowertrack.Domain.Entities.Organizations;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Organization aggregate.
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// Gets an organization by its name.
    /// </summary>
    /// <param name="name">The organization name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The organization if found; otherwise, null.</returns>
    Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an organization with all its machines loaded.
    /// </summary>
    /// <param name="id">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The organization with machines if found; otherwise, null.</returns>
    Task<Organization?> GetWithMachinesAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an organization with all its contacts loaded.
    /// </summary>
    /// <param name="id">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The organization with contacts if found; otherwise, null.</returns>
    Task<Organization?> GetWithContactsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active organizations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all active organizations.</returns>
    Task<List<Organization>> GetActiveOrganizationsAsync(CancellationToken cancellationToken = default);
}
