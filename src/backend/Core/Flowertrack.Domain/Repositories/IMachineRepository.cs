using Flowertrack.Domain.Entities;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Machine aggregate.
/// </summary>
public interface IMachineRepository : IRepository<Machine>
{
    /// <summary>
    /// Gets a machine by its serial number.
    /// </summary>
    Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all machines for a specific organization.
    /// </summary>
    Task<List<Machine>> GetByOrganizationAsync(Guid orgId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a machine by its API key for machine-initiated requests.
    /// </summary>
    Task<Machine?> GetByApiTokenAsync(MachineApiKey token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all active machines.
    /// </summary>
    Task<List<Machine>> GetActiveMachinesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets machines that are due for maintenance within specified days.
    /// </summary>
    Task<List<Machine>> GetMachinesDueForMaintenanceAsync(int daysThreshold, CancellationToken cancellationToken = default);
}
