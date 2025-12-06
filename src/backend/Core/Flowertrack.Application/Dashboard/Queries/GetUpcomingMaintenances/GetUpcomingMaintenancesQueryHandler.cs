using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Flowertrack.Application.Dashboard.Queries.GetUpcomingMaintenances;

/// <summary>
/// Handler for GetUpcomingMaintenancesQuery
/// US-030: Konfiguracja przeglądów technicznych
/// </summary>
public sealed class GetUpcomingMaintenancesQueryHandler
    : IRequestHandler<GetUpcomingMaintenancesQuery, Result<UpcomingMaintenancesDto>>
{
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetUpcomingMaintenancesQueryHandler> _logger;

    public GetUpcomingMaintenancesQueryHandler(
        IMachineRepository machineRepository,
        IOrganizationRepository organizationRepository,
        ILogger<GetUpcomingMaintenancesQueryHandler> logger)
    {
        _machineRepository = machineRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<UpcomingMaintenancesDto>> Handle(
        GetUpcomingMaintenancesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching upcoming maintenances, DaysAhead: {DaysAhead}, Limit: {Limit}, Organization: {OrganizationId}, User: {UserId}",
                request.DaysAhead,
                request.Limit,
                request.OrganizationId,
                request.RequestedBy);

            var daysAhead = Math.Max(1, Math.Min(request.DaysAhead, 365)); // Max 1 year ahead
            var limit = Math.Max(1, Math.Min(request.Limit, 100)); // Max 100 items

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(daysAhead);

            // Get organization names lookup
            var organizations = await _organizationRepository.GetAllAsync(cancellationToken);
            var orgDict = organizations.ToDictionary(o => o.Id, o => o.Name);

            // Get all machines
            var allMachines = await _machineRepository.GetAllAsync(cancellationToken);

            // Filter machines with scheduled maintenance in the date range
            var upcomingMaintenances = allMachines
                .Where(m => !m.IsDeleted)
                .Where(m => request.OrganizationId == null || m.OrganizationId == request.OrganizationId)
                .Where(m => m.NextMaintenanceDate.HasValue)
                .Where(m => m.NextMaintenanceDate!.Value >= today && m.NextMaintenanceDate.Value <= endDate)
                .OrderBy(m => m.NextMaintenanceDate)
                .Select(m => new UpcomingMaintenanceDto
                {
                    MachineId = m.Id,
                    SerialNumber = m.SerialNumber,
                    MachineName = !string.IsNullOrEmpty(m.Model)
                        ? $"{m.Brand ?? ""} {m.Model}".Trim()
                        : m.SerialNumber,
                    OrganizationName = orgDict.GetValueOrDefault(m.OrganizationId, "Unknown"),
                    ScheduledDate = new DateTimeOffset(m.NextMaintenanceDate!.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
                    MaintenanceType = "Scheduled Maintenance"
                })
                .ToList();

            var result = new UpcomingMaintenancesDto
            {
                Maintenances = upcomingMaintenances.Take(limit).ToList(),
                TotalCount = upcomingMaintenances.Count
            };

            _logger.LogInformation(
                "Retrieved {Count} upcoming maintenances (total: {TotalCount})",
                result.Maintenances.Count,
                result.TotalCount);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching upcoming maintenances");
            return Result.Failure<UpcomingMaintenancesDto>($"Failed to fetch upcoming maintenances: {ex.Message}");
        }
    }
}
