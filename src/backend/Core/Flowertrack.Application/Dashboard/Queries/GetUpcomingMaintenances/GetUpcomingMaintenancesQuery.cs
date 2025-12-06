using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using MediatR;

namespace Flowertrack.Application.Dashboard.Queries.GetUpcomingMaintenances;

/// <summary>
/// Query to retrieve upcoming scheduled maintenances
/// US-030: Konfiguracja przeglądów technicznych
/// </summary>
public sealed record GetUpcomingMaintenancesQuery : IRequest<Result<UpcomingMaintenancesDto>>
{
    /// <summary>
    /// Number of days ahead to look for maintenances (default 30)
    /// </summary>
    public int DaysAhead { get; init; } = 30;

    /// <summary>
    /// Maximum number of maintenances to return (default 20)
    /// </summary>
    public int Limit { get; init; } = 20;

    /// <summary>
    /// Optional organization ID to filter maintenances (null for all organizations)
    /// </summary>
    public Guid? OrganizationId { get; init; }

    /// <summary>
    /// User ID making the request (for authorization)
    /// </summary>
    public Guid RequestedBy { get; init; }
}

/// <summary>
/// DTO containing upcoming maintenances list
/// </summary>
public sealed record UpcomingMaintenancesDto
{
    /// <summary>
    /// List of upcoming scheduled maintenances
    /// </summary>
    public IReadOnlyList<UpcomingMaintenanceDto> Maintenances { get; init; } = [];

    /// <summary>
    /// Total count of upcoming maintenances in the period
    /// </summary>
    public int TotalCount { get; init; }
}
