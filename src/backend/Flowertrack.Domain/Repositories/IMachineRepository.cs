using Flowertrack.Domain.Entities.Machines;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Machine aggregate.
/// </summary>
public interface IMachineRepository : IRepository<Machine>
{
    /// <summary>
    /// Gets a machine by its serial number.
    /// </summary>
    /// <param name="serialNumber">The machine serial number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The machine if found; otherwise, null.</returns>
    Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all machines for a specific organization.
    /// </summary>
    /// <param name="orgId">The organization identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of machines for the organization.</returns>
    Task<List<Machine>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a machine by its API key for machine-initiated requests.
    /// </summary>
    /// <param name="token">The machine API key/token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The machine if found; otherwise, null.</returns>
    Task<Machine?> GetByApiTokenAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active machines.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all active machines.</returns>
    Task<List<Machine>> GetActiveMachinesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets machines that are due for maintenance within specified days.
    /// </summary>
    /// <param name="daysThreshold">The number of days threshold for maintenance due date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of machines due for maintenance.</returns>
    Task<List<Machine>> GetMachinesDueForMaintenanceAsync(int daysThreshold, CancellationToken cancellationToken = default);
}
