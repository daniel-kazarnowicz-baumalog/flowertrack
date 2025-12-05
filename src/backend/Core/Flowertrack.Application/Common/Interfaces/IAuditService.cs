using Flowertrack.Domain.Entities;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for creating audit log entries to track user actions in the system.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs a successful action.
    /// </summary>
    /// <param name="actionType">Type of action (use AuditActionTypes constants)</param>
    /// <param name="resourceType">Type of affected resource (e.g., "Ticket", "Machine")</param>
    /// <param name="resourceId">ID of the affected resource</param>
    /// <param name="description">Human-readable description of the action</param>
    /// <param name="oldValue">Previous state as JSON (for updates)</param>
    /// <param name="newValue">New state as JSON (for creates/updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogAsync(
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a failed action.
    /// </summary>
    /// <param name="actionType">Type of action that failed</param>
    /// <param name="errorMessage">Error message describing the failure</param>
    /// <param name="resourceType">Type of affected resource</param>
    /// <param name="resourceId">ID of the affected resource</param>
    /// <param name="description">Human-readable description</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogFailureAsync(
        string actionType,
        string errorMessage,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a login attempt.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="isSuccess">Whether login was successful</param>
    /// <param name="errorMessage">Error message if login failed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogLoginAsync(
        string email,
        bool isSuccess,
        string? errorMessage = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a logout action.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogLogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit log entry without saving (for batch operations).
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    AuditLog CreateEntry(
        string actionType,
        string? resourceType = null,
        string? resourceId = null,
        string? description = null,
        string? oldValue = null,
        string? newValue = null,
        bool isSuccess = true,
        string? errorMessage = null);
}
