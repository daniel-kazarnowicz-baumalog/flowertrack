using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for MachineLog entity.
/// Provides specialized query methods for machine log management.
/// </summary>
public interface IMachineLogRepository : IRepository<MachineLog>
{
    /// <summary>
    /// Gets all logs for a specific machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machine logs.</returns>
    Task<IReadOnlyList<MachineLog>> GetByMachineIdAsync(Guid machineId, CancellationToken ct = default);

    /// <summary>
    /// Gets logs for a machine within a date range.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="from">Start of date range.</param>
    /// <param name="to">End of date range.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machine logs.</returns>
    Task<IReadOnlyList<MachineLog>> GetByMachineIdAndDateRangeAsync(
        Guid machineId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default);

    /// <summary>
    /// Gets alarm logs for a machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of alarm logs.</returns>
    Task<IReadOnlyList<MachineLog>> GetAlarmsByMachineIdAsync(Guid machineId, CancellationToken ct = default);

    /// <summary>
    /// Gets unprocessed logs.
    /// </summary>
    /// <param name="limit">Maximum number of logs to return.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of unprocessed logs.</returns>
    Task<IReadOnlyList<MachineLog>> GetUnprocessedAsync(int limit = 100, CancellationToken ct = default);

    /// <summary>
    /// Gets the latest log for a machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The latest log if exists; otherwise, null.</returns>
    Task<MachineLog?> GetLatestByMachineIdAsync(Guid machineId, CancellationToken ct = default);

    /// <summary>
    /// Gets paginated logs for a machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of machine logs for the page.</returns>
    Task<IReadOnlyList<MachineLog>> GetByMachineIdPagedAsync(
        Guid machineId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Gets total count of logs for a machine.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Total count of logs.</returns>
    Task<int> GetCountByMachineIdAsync(Guid machineId, CancellationToken ct = default);
}
