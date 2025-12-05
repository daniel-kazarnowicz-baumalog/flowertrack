using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Audit.Queries.GetAuditLogs;

/// <summary>
/// Query to retrieve audit logs with filtering and pagination.
/// US-054: Pełny audyt akcji użytkowników
/// </summary>
public sealed record GetAuditLogsQuery : IRequest<Result<PagedResult<AuditLogDto>>>
{
    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Filter by user ID (optional)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Filter by action type (optional, e.g., "Create", "Update", "Delete")
    /// </summary>
    public string? ActionType { get; init; }

    /// <summary>
    /// Filter by resource type (optional, e.g., "Ticket", "Machine", "User")
    /// </summary>
    public string? ResourceType { get; init; }

    /// <summary>
    /// Filter by resource ID (optional)
    /// </summary>
    public string? ResourceId { get; init; }

    /// <summary>
    /// Filter by start date (optional)
    /// </summary>
    public DateTimeOffset? FromDate { get; init; }

    /// <summary>
    /// Filter by end date (optional)
    /// </summary>
    public DateTimeOffset? ToDate { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default 50, max 100)
    /// </summary>
    public int PageSize { get; init; } = 50;
}
