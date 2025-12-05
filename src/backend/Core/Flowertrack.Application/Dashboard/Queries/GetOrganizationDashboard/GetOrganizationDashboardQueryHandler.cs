using Flowertrack.Application.Common.Models;
using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using MachineStatus = Flowertrack.Domain.ValueObjects.MachineStatus;

namespace Flowertrack.Application.Dashboard.Queries.GetOrganizationDashboard;

/// <summary>
/// Handler for GetOrganizationDashboardQuery that aggregates KPIs for a specific organization.
/// Used by OrganizationAdministrator and Operator users.
/// </summary>
public sealed class GetOrganizationDashboardQueryHandler
    : IRequestHandler<GetOrganizationDashboardQuery, Result<OrganizationDashboardDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationUserRepository _organizationUserRepository;
    private readonly ILogger<GetOrganizationDashboardQueryHandler> _logger;

    public GetOrganizationDashboardQueryHandler(
        ITicketRepository ticketRepository,
        IMachineRepository machineRepository,
        IOrganizationRepository organizationRepository,
        IOrganizationUserRepository organizationUserRepository,
        ILogger<GetOrganizationDashboardQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _machineRepository = machineRepository;
        _organizationRepository = organizationRepository;
        _organizationUserRepository = organizationUserRepository;
        _logger = logger;
    }

    public async Task<Result<OrganizationDashboardDto>> Handle(
        GetOrganizationDashboardQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Generating organization dashboard for user {UserId}, org {OrgId}",
                request.RequestedBy,
                request.OrganizationId);

            // Determine which organization to show
            Guid organizationId;
            if (request.OrganizationId.HasValue)
            {
                organizationId = request.OrganizationId.Value;
            }
            else
            {
                // Get user's organization
                var orgUser = await _organizationUserRepository.GetByIdAsync(
                    request.RequestedBy,
                    cancellationToken);

                if (orgUser == null)
                {
                    _logger.LogWarning(
                        "User {UserId} not found in any organization",
                        request.RequestedBy);
                    return Result.Failure<OrganizationDashboardDto>("User not associated with any organization");
                }

                organizationId = orgUser.OrganizationId;
            }

            // Get organization details
            var organization = await _organizationRepository.GetByIdAsync(organizationId, cancellationToken);
            if (organization == null || organization.IsDeleted)
            {
                return Result.Failure<OrganizationDashboardDto>("Organization not found");
            }

            var now = DateTimeOffset.UtcNow;
            var yesterday = now.AddHours(-24);

            // Get machines for this organization
            var allMachines = await _machineRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);
            var activeMachines = allMachines.Where(m => !m.IsDeleted).ToList();

            // Machine KPIs - using ValueObjects.MachineStatus
            var totalActiveMachines = activeMachines.Count;
            var operationalMachines = activeMachines.Count(m => m.Status == MachineStatus.Active);
            var machinesWithAlarms = activeMachines.Count(m => m.Status == MachineStatus.Alarm);
            var machinesInMaintenance = activeMachines.Count(m => m.Status == MachineStatus.Maintenance);
            var machinesInactive = activeMachines.Count(m => m.Status == MachineStatus.Inactive);

            // Get tickets for this organization
            var allTickets = await _ticketRepository.GetAllAsync(cancellationToken);
            var orgTickets = allTickets
                .Where(t => !t.IsDeleted && t.OrganizationId == organizationId)
                .ToList();

            // Ticket KPIs - active statuses: New, Accepted, InProgress, Reopened
            var activeStatuses = new[] { TicketStatus.New, TicketStatus.Accepted, TicketStatus.InProgress, TicketStatus.Reopened };
            var myActiveTickets = orgTickets.Count(t =>
                activeStatuses.Contains(t.Status) &&
                t.CreatedByUserId == request.RequestedBy);

            var allActiveTickets = orgTickets.Count(t => activeStatuses.Contains(t.Status));

            var resolvedLast24h = orgTickets.Count(t =>
                t.Status == TicketStatus.Resolved &&
                t.UpdatedAt.HasValue &&
                t.UpdatedAt.Value >= yesterday);

            var criticalTickets = orgTickets.Count(t =>
                activeStatuses.Contains(t.Status) &&
                t.Priority == Priority.Critical);

            // Machine alarms
            var machineAlarms = activeMachines
                .Where(m => m.Status == MachineStatus.Alarm)
                .Select(m => new MachineAlarmDto
                {
                    MachineId = m.Id,
                    SerialNumber = m.SerialNumber,
                    MachineName = $"{m.Brand} {m.Model}".Trim(),
                    Location = m.Location ?? string.Empty,
                    Status = m.Status.ToString(),
                    AlarmStartTime = m.UpdatedAt
                })
                .OrderByDescending(m => m.AlarmStartTime)
                .Take(10)
                .ToList();

            // Recent activities
            var recentActivities = orgTickets
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Take(15)
                .Select(t => new RecentActivityDto
                {
                    Id = t.Id,
                    ActivityType = GetActivityType(t, yesterday),
                    Description = GetActivityDescription(t),
                    RelatedEntity = t.Title,
                    OrganizationName = organization.Name,
                    Timestamp = t.UpdatedAt ?? t.CreatedAt,
                    PerformedBy = "System"
                })
                .ToList();

            var dashboard = new OrganizationDashboardDto
            {
                OrganizationId = organizationId,
                OrganizationName = organization.Name,
                ActiveMachines = totalActiveMachines,
                OperationalMachines = operationalMachines,
                MachinesWithAlarms = machinesWithAlarms,
                MachinesInMaintenance = machinesInMaintenance,
                MachinesWithWarnings = machinesInactive, // repurposed for inactive count
                MyActiveTickets = myActiveTickets,
                AllOrganizationActiveTickets = allActiveTickets,
                ResolvedTicketsLast24h = resolvedLast24h,
                CriticalTickets = criticalTickets,
                RecentActivities = recentActivities,
                MachineAlarms = machineAlarms
            };

            _logger.LogInformation(
                "Organization dashboard generated for {OrgName}: {Machines} machines, {Tickets} active tickets",
                organization.Name,
                totalActiveMachines,
                allActiveTickets);

            return Result.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating organization dashboard");
            return Result.Failure<OrganizationDashboardDto>("Failed to generate organization dashboard");
        }
    }

    private static string GetActivityType(Domain.Entities.Ticket ticket, DateTimeOffset yesterday)
    {
        if (ticket.CreatedAt >= yesterday)
            return "TicketCreated";

        return ticket.Status switch
        {
            TicketStatus.Resolved => "TicketResolved",
            TicketStatus.Closed => "TicketClosed",
            TicketStatus.InProgress => "TicketInProgress",
            _ => "TicketUpdated"
        };
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
