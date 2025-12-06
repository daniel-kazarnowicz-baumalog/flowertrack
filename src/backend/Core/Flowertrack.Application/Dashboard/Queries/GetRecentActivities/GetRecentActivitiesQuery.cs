using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetRecentActivities;

/// <summary>
/// Query to retrieve recent activities across the system
/// US-008: Lista ostatnich zdarzeń
/// </summary>
public sealed record GetRecentActivitiesQuery : IRequest<Result<RecentActivitiesDto>>
{
    /// <summary>
    /// Maximum number of activities to return (default 10)
    /// </summary>
    public int Limit { get; init; } = 10;

    /// <summary>
    /// Optional organization ID to filter activities (null for all organizations)
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}

/// <summary>
/// DTO containing recent activities list
/// </summary>
public sealed record RecentActivitiesDto
{
    /// <summary>
    /// List of recent activities
    /// </summary>
    public IReadOnlyList<RecentActivityDto> Activities { get; init; } = [];
}
