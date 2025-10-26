using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for Machine aggregate root.
/// Provides specialized query methods for machine management.
/// </summary>
public interface IMachineRepository : IRepository<Machine>
{
    /// <summary>
    /// Gets a machine by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The machine if found; otherwise, null.</returns>
    Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);

    /// <summary>
    /// Gets a machine by its API token.
    /// </summary>
    /// <param name="apiToken">The API token.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The machine if found; otherwise, null.</returns>
    Task<Machine?> GetByApiTokenAsync(string apiToken, CancellationToken ct = default);

    /// <summary>
    /// Gets all machines for a specific organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machines.</returns>
    Task<IReadOnlyList<Machine>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default);

    /// <summary>
    /// Gets all machines with a specific status.
    /// </summary>
    /// <param name="status">The machine status to filter by.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machines.</returns>
    Task<IReadOnlyList<Machine>> GetByStatusAsync(MachineStatus status, CancellationToken ct = default);

    /// <summary>
    /// Checks if a serial number already exists.
    /// </summary>
    /// <param name="serialNumber">The serial number to check.</param>
    /// <param name="excludeId">Optional machine ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the serial number exists; otherwise, false.</returns>
    Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId = null, CancellationToken ct = default);

    /// <summary>
    /// Checks if an API token already exists.
    /// </summary>
    /// <param name="apiToken">The API token to check.</param>
    /// <param name="excludeId">Optional machine ID to exclude from the check (for updates).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the API token exists; otherwise, false.</returns>
    Task<bool> ApiTokenExistsAsync(string apiToken, Guid? excludeId = null, CancellationToken ct = default);

    /// <summary>
    /// Gets all machines that are due for maintenance before the specified date.
    /// </summary>
    /// <param name="beforeDate">The date threshold.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machines due for maintenance.</returns>
    Task<IReadOnlyList<Machine>> GetMachinesDueForMaintenanceAsync(DateOnly beforeDate, CancellationToken ct = default);
}
