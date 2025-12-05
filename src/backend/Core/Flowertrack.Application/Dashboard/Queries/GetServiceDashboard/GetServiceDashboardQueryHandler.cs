using Flowertrack.Application.Common.Models;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using MachineStatus = Flowertrack.Domain.ValueObjects.MachineStatus;

namespace Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;

/// <summary>
/// Handler for GetServiceDashboardQuery that aggregates KPIs across all organizations.
/// Used by ServiceAdministrator and ServiceTechnician users.
/// </summary>
public sealed class GetServiceDashboardQueryHandler
    : IRequestHandler<GetServiceDashboardQuery, Result<ServiceDashboardDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetServiceDashboardQueryHandler> _logger;

    public GetServiceDashboardQueryHandler(
        ITicketRepository ticketRepository,
        IMachineRepository machineRepository,
        IOrganizationRepository organizationRepository,
        ILogger<GetServiceDashboardQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _machineRepository = machineRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<Result<ServiceDashboardDto>> Handle(
        GetServiceDashboardQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Generating service dashboard for user {UserId}",
                request.RequestedBy);

            var now = DateTimeOffset.UtcNow;
            var yesterday = now.AddHours(-24);

            // Get all tickets
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);
            var activeTickets = allTickets.Where(t => !t.IsDeleted).ToList();

            // Calculate ticket KPIs
            // Active statuses: New, Accepted, InProgress, Reopened
            var activeStatuses = new[] { TicketStatus.New, TicketStatus.Accepted, TicketStatus.InProgress, TicketStatus.Reopened };
            var totalActiveTickets = activeTickets.Count(t => activeStatuses.Contains(t.Status));
            var criticalTickets = activeTickets.Count(t =>
                activeStatuses.Contains(t.Status) && t.Priority == Priority.Critical);
            var highPriorityTickets = activeTickets.Count(t =>
                activeStatuses.Contains(t.Status) && t.Priority == Priority.High);

            var resolvedLast24h = activeTickets.Count(t =>
                t.Status == TicketStatus.Resolved &&
                t.UpdatedAt.HasValue &&
                t.UpdatedAt.Value >= yesterday);

            var closedLast24h = activeTickets.Count(t =>
                t.Status == TicketStatus.Closed &&
                t.UpdatedAt.HasValue &&
                t.UpdatedAt.Value >= yesterday);

            var newTicketsLast24h = activeTickets.Count(t => t.CreatedAt >= yesterday);

            // Get all machines
            var allMachines = await _machineRepository.GetAllAsync(cancellationToken);
            var activeMachines = allMachines.Where(m => !m.IsDeleted).ToList();

            // Machine KPIs - using ValueObjects.MachineStatus
            var machinesWithAlarms = activeMachines.Count(m =>
                m.Status == MachineStatus.Alarm);
            var machinesInMaintenance = activeMachines.Count(m =>
                m.Status == MachineStatus.Maintenance);

            // Organizations with alarms
            var organizations = await _organizationRepository.GetAllAsync(cancellationToken);
            var orgDict = organizations.ToDictionary(o => o.Id, o => o.Name);

            var orgsWithAlarms = activeMachines
                .Where(m => m.Status == MachineStatus.Alarm)
                .GroupBy(m => m.OrganizationId)
                .Select(g => new OrganizationAlarmDto
                {
                    OrganizationId = g.Key,
                    OrganizationName = orgDict.GetValueOrDefault(g.Key, "Unknown"),
                    MachinesWithAlarms = g.Count(),
                    HighestAlarmLevel = "CRITICAL",
                    ActiveAlarmCount = g.Count()
                })
                .OrderByDescending(o => o.MachinesWithAlarms)
                .Take(10)
                .ToList();

            // Upcoming maintenances (machines in maintenance status)
            var upcomingMaintenances = activeMachines
                .Where(m => m.Status == MachineStatus.Maintenance)
                .Select(m => new UpcomingMaintenanceDto
                {
                    MachineId = m.Id,
                    SerialNumber = m.SerialNumber,
                    MachineName = $"{m.Brand} {m.Model}".Trim(),
                    OrganizationName = orgDict.GetValueOrDefault(m.OrganizationId, "Unknown"),
                    ScheduledDate = m.UpdatedAt ?? m.CreatedAt,
                    MaintenanceType = "Scheduled"
                })
                .OrderBy(m => m.ScheduledDate)
                .Take(10)
                .ToList();

            // Recent activities (based on recent tickets)
            var recentActivities = activeTickets
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Take(20)
                .Select(t => new RecentActivityDto
                {
                    Id = t.Id,
                    ActivityType = t.CreatedAt >= yesterday ? "TicketCreated" : "TicketUpdated",
                    Description = GetActivityDescription(t),
                    RelatedEntity = t.Title,
                    OrganizationName = orgDict.GetValueOrDefault(t.OrganizationId, "Unknown"),
                    Timestamp = t.UpdatedAt ?? t.CreatedAt,
                    PerformedBy = "System"
                })
                .ToList();

            var dashboard = new ServiceDashboardDto
            {
                TotalActiveTickets = totalActiveTickets,
                CriticalTickets = criticalTickets,
                HighPriorityTickets = highPriorityTickets,
                ResolvedLast24h = resolvedLast24h,
                ClosedLast24h = closedLast24h,
                NewTicketsLast24h = newTicketsLast24h,
                TotalMachinesWithAlarms = machinesWithAlarms,
                TotalMachinesInMaintenance = machinesInMaintenance,
                RecentActivities = recentActivities,
                OrganizationsWithAlarms = orgsWithAlarms,
                UpcomingMaintenances = upcomingMaintenances
            };

            _logger.LogInformation(
                "Service dashboard generated: {ActiveTickets} active tickets, {AlarmMachines} machines with alarms",
                totalActiveTickets,
                machinesWithAlarms);

            return Result.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating service dashboard");
            return Result.Failure<ServiceDashboardDto>("Failed to generate service dashboard");
        }
    }

    private static string GetActivityDescription(Domain.Entities.Ticket ticket)
    {
        return ticket.Status switch
        {
            TicketStatus.New => $"New ticket: {ticket.Title}",
            TicketStatus.Accepted => $"Accepted: {ticket.Title}",
            TicketStatus.InProgress => $"In progress: {ticket.Title}",
            TicketStatus.Resolved => $"Resolved: {ticket.Title}",
            TicketStatus.Closed => $"Closed: {ticket.Title}",
            TicketStatus.Reopened => $"Reopened: {ticket.Title}",
            _ => $"Updated: {ticket.Title}"
        };
    }
}
