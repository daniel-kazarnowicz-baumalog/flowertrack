using Flowertrack.Application.Common.Models;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetOrganizationDashboard;

/// <summary>
/// Query to retrieve the organization dashboard KPIs and activities.
/// US-007: Przeglądanie KPI organizacji
/// </summary>
public sealed record GetOrganizationDashboardQuery : IRequest<Result<OrganizationDashboardDto>>
{
    /// <summary>
    /// User ID making the request (for authorization and filtering)
    /// </summary>
    public Guid RequestedBy { get; init; }

    /// <summary>
    /// Organization ID to get dashboard for. If null, uses user's organization.
    /// </summary>
    public Guid? OrganizationId { get; init; }
}
