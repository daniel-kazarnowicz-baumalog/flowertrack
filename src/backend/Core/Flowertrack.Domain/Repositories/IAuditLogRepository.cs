using Flowertrack.Domain.Entities;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for AuditLog entity.
/// Provides specialized query methods for audit log management.
/// Note: AuditLog does not support Update or Delete operations by design.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Adds a new audit log entry.
    /// </summary>
    /// <param name="auditLog">The audit log to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added audit log with generated ID.</returns>
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken ct = default);

    /// <summary>
    /// Adds multiple audit log entries.
    /// </summary>
    /// <param name="auditLogs">The audit logs to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddRangeAsync(IEnumerable<AuditLog> auditLogs, CancellationToken ct = default);

    /// <summary>
    /// Gets an audit log by its identifier.
    /// </summary>
    /// <param name="id">The audit log identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The audit log if found; otherwise, null.</returns>
    Task<AuditLog?> GetByIdAsync(long id, CancellationToken ct = default);

    /// <summary>
    /// Gets audit logs for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of audit logs.</returns>
    Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Gets audit logs for a specific resource.
    /// </summary>
    /// <param name="resourceType">The resource type.</param>
    /// <param name="resourceId">The resource identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of audit logs.</returns>
    Task<IReadOnlyList<AuditLog>> GetByResourceAsync(
        string resourceType,
        string resourceId,
        CancellationToken ct = default);

    /// <summary>
    /// Gets audit logs by action type.
    /// </summary>
    /// <param name="actionType">The action type.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of audit logs.</returns>
    Task<IReadOnlyList<AuditLog>> GetByActionTypeAsync(
        string actionType,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Gets audit logs within a date range.
    /// </summary>
    /// <param name="from">Start of date range.</param>
    /// <param name="to">End of date range.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of audit logs.</returns>
    Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Searches audit logs with multiple filters.
    /// </summary>
    /// <param name="userId">Optional user ID filter.</param>
    /// <param name="actionType">Optional action type filter.</param>
    /// <param name="resourceType">Optional resource type filter.</param>
    /// <param name="from">Optional start date filter.</param>
    /// <param name="to">Optional end date filter.</param>
    /// <param name="isSuccess">Optional success status filter.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of audit logs.</returns>
    Task<IReadOnlyList<AuditLog>> SearchAsync(
        Guid? userId = null,
        string? actionType = null,
        string? resourceType = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        bool? isSuccess = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    /// <summary>
    /// Gets total count of audit logs matching the search criteria.
    /// </summary>
    Task<int> GetSearchCountAsync(
        Guid? userId = null,
        string? actionType = null,
        string? resourceType = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        bool? isSuccess = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets failed login attempts for a user within a time window.
    /// </summary>
    /// <param name="userEmail">The user email.</param>
    /// <param name="since">Time window start.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Count of failed login attempts.</returns>
    Task<int> GetFailedLoginCountAsync(
        string userEmail,
        DateTimeOffset since,
        CancellationToken ct = default);
}
