using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetOrganizationActivities;

/// <summary>
/// Query to retrieve recent activities for a specific organization
/// US-038: Ostatnie aktywności (Portal Klienta)
/// </summary>
public sealed record GetOrganizationActivitiesQuery : IRequest<Result<OrganizationActivitiesDto>>
{
    /// <summary>
    /// Organization ID to get activities for
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Maximum number of activities to return (default 10)
    /// </summary>
    public int Limit { get; init; } = 10;

    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}

/// <summary>
/// DTO containing organization activities
/// </summary>
public sealed record OrganizationActivitiesDto
{
    /// <summary>
    /// Organization ID
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrganizationName { get; init; } = string.Empty;

    /// <summary>
    /// List of recent activities
    /// </summary>
    public IReadOnlyList<RecentActivityDto> Activities { get; init; } = [];
}
