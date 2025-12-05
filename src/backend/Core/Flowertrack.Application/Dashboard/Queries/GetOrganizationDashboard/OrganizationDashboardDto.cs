using Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;

namespace Flowertrack.Application.Dashboard.Queries.GetOrganizationDashboard;

/// <summary>
/// DTO containing organization dashboard KPIs and recent activities.
/// Used by OrganizationAdministrator and Operator users.
/// </summary>
public sealed record OrganizationDashboardDto
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
    /// Total number of active (non-deleted) machines
    /// </summary>
    public int ActiveMachines { get; init; }

    /// <summary>
    /// Number of machines currently operational
    /// </summary>
    public int OperationalMachines { get; init; }

    /// <summary>
    /// Number of machines with active alarms (Error status)
    /// </summary>
    public int MachinesWithAlarms { get; init; }

    /// <summary>
    /// Number of machines in maintenance mode
    /// </summary>
    public int MachinesInMaintenance { get; init; }

    /// <summary>
    /// Number of machines with warning status
    /// </summary>
    public int MachinesWithWarnings { get; init; }

    /// <summary>
    /// Number of tickets created by the current user that are active
    /// </summary>
    public int MyActiveTickets { get; init; }

    /// <summary>
    /// Total number of active tickets for the organization
    /// </summary>
    public int AllOrganizationActiveTickets { get; init; }

    /// <summary>
    /// Number of tickets resolved in the last 24 hours
    /// </summary>
    public int ResolvedTicketsLast24h { get; init; }

    /// <summary>
    /// Number of tickets with Critical priority
    /// </summary>
    public int CriticalTickets { get; init; }

    /// <summary>
    /// Recent activities for this organization
    /// </summary>
    public List<RecentActivityDto> RecentActivities { get; init; } = [];

    /// <summary>
    /// List of machines with current alarms
    /// </summary>
    public List<MachineAlarmDto> MachineAlarms { get; init; } = [];
}

/// <summary>
/// DTO for machine alarm information
/// </summary>
public sealed record MachineAlarmDto
{
    /// <summary>
    /// Machine ID
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// Machine serial number
    /// </summary>
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// Machine name (brand + model)
    /// </summary>
    public string MachineName { get; init; } = string.Empty;

    /// <summary>
    /// Machine location
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Alarm code if available
    /// </summary>
    public string? AlarmCode { get; init; }

    /// <summary>
    /// When the alarm started
    /// </summary>
    public DateTimeOffset? AlarmStartTime { get; init; }
}
