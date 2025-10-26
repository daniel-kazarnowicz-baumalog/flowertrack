using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Organization aggregate.
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// Gets an organization by its name.
    /// </summary>
    Task<Organization?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an organization with all its machines loaded.
    /// </summary>
    Task<Organization?> GetWithMachinesAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an organization with all its contacts loaded.
    /// </summary>
    Task<Organization?> GetWithContactsAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active organizations.
    /// </summary>
    Task<List<Organization>> GetActiveOrganizationsAsync(CancellationToken cancellationToken = default);
}
