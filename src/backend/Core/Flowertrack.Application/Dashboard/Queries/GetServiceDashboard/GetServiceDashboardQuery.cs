using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;

/// <summary>
/// Query to retrieve the service dashboard KPIs and recent activities.
/// US-006: Przeglądanie KPI serwisu
/// US-037: Dashboard serwisu
/// </summary>
public sealed record GetServiceDashboardQuery : IRequest<Result<ServiceDashboardDto>>
{
    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}
