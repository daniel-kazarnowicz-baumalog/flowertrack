namespace Flowertrack.Application.Audit.Queries.GetAuditLogs;

/// <summary>
/// DTO representing an audit log entry
/// </summary>
public sealed record AuditLogDto
{
    /// <summary>
    /// Unique identifier of the audit log entry
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// ID of the user who performed the action (null for system actions)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Email of the user who performed the action
    /// </summary>
    public string? UserEmail { get; init; }

    /// <summary>
    /// Type of action performed (e.g., "Create", "Update", "Delete", "Login")
    /// </summary>
    public string ActionType { get; init; } = string.Empty;

    /// <summary>
    /// Type of resource affected (e.g., "Ticket", "Machine", "Organization")
    /// </summary>
    public string ResourceType { get; init; } = string.Empty;

    /// <summary>
    /// ID of the resource affected
    /// </summary>
    public string? ResourceId { get; init; }

    /// <summary>
    /// JSON representation of the old values (for updates)
    /// </summary>
    public string? OldValue { get; init; }

    /// <summary>
    /// JSON representation of the new values (for creates/updates)
    /// </summary>
    public string? NewValue { get; init; }

    /// <summary>
    /// IP address of the request
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// User agent of the request
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// When the action was performed
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
}
