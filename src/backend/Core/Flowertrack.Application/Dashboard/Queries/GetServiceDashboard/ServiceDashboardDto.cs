namespace Flowertrack.Application.Dashboard.Queries.GetServiceDashboard;

/// <summary>
/// DTO containing service dashboard KPIs and recent activities.
/// Used by ServiceAdministrator and ServiceTechnician users.
/// </summary>
public sealed record ServiceDashboardDto
{
    /// <summary>
    /// Total number of active tickets (Open, InProgress, Pending)
    /// </summary>
    public int TotalActiveTickets { get; init; }

    /// <summary>
    /// Number of tickets with Critical priority
    /// </summary>
    public int CriticalTickets { get; init; }

    /// <summary>
    /// Number of tickets with High priority
    /// </summary>
    public int HighPriorityTickets { get; init; }

    /// <summary>
    /// Number of tickets resolved in the last 24 hours
    /// </summary>
    public int ResolvedLast24h { get; init; }

    /// <summary>
    /// Number of tickets closed in the last 24 hours
    /// </summary>
    public int ClosedLast24h { get; init; }

    /// <summary>
    /// Number of new tickets created in the last 24 hours
    /// </summary>
    public int NewTicketsLast24h { get; init; }

    /// <summary>
    /// Total number of machines in alarm state across all organizations
    /// </summary>
    public int TotalMachinesWithAlarms { get; init; }

    /// <summary>
    /// Total number of machines in maintenance mode
    /// </summary>
    public int TotalMachinesInMaintenance { get; init; }

    /// <summary>
    /// Recent activities across all organizations
    /// </summary>
    public List<RecentActivityDto> RecentActivities { get; init; } = [];

    /// <summary>
    /// Organizations with active machine alarms
    /// </summary>
    public List<OrganizationAlarmDto> OrganizationsWithAlarms { get; init; } = [];

    /// <summary>
    /// Upcoming scheduled maintenances
    /// </summary>
    public List<UpcomingMaintenanceDto> UpcomingMaintenances { get; init; } = [];

    /// <summary>
    /// Ticket trends for the last 30 days (for line chart)
    /// </summary>
    public List<TicketTrendDataPointDto> TicketTrends { get; init; } = [];

    /// <summary>
    /// Distribution of active tickets by priority (for pie/bar chart)
    /// </summary>
    public List<PriorityDistributionDto> PriorityDistribution { get; init; } = [];
}

/// <summary>
/// DTO for recent activity items
/// </summary>
public sealed record RecentActivityDto
{
    /// <summary>
    /// Activity ID (could be Ticket ID, Comment ID, etc.)
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Type of activity (TicketCreated, CommentAdded, StatusChanged, etc.)
    /// </summary>
    public string ActivityType { get; init; } = string.Empty;

    /// <summary>
    /// Description of the activity
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Related entity (Ticket title, Machine name, etc.)
    /// </summary>
    public string RelatedEntity { get; init; } = string.Empty;

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrganizationName { get; init; } = string.Empty;

    /// <summary>
    /// When the activity occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// User who performed the activity
    /// </summary>
    public string PerformedBy { get; init; } = string.Empty;
}

/// <summary>
/// DTO for organization alarm summary
/// </summary>
public sealed record OrganizationAlarmDto
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
    /// Number of machines in alarm state
    /// </summary>
    public int MachinesWithAlarms { get; init; }

    /// <summary>
    /// Most critical alarm level (CRITICAL, HIGH, MEDIUM, LOW)
    /// </summary>
    public string HighestAlarmLevel { get; init; } = string.Empty;

    /// <summary>
    /// Total number of active alarms
    /// </summary>
    public int ActiveAlarmCount { get; init; }
}

/// <summary>
/// DTO for upcoming maintenance items
/// </summary>
public sealed record UpcomingMaintenanceDto
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
    /// Machine name/model
    /// </summary>
    public string MachineName { get; init; } = string.Empty;

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrganizationName { get; init; } = string.Empty;

    /// <summary>
    /// Scheduled maintenance date
    /// </summary>
    public DateTimeOffset ScheduledDate { get; init; }

    /// <summary>
    /// Type of maintenance
    /// </summary>
    public string MaintenanceType { get; init; } = string.Empty;
}

/// <summary>
/// DTO for ticket trend data point (for line chart)
/// </summary>
public sealed record TicketTrendDataPointDto
{
    /// <summary>
    /// Date of the data point
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Number of tickets created on this date
    /// </summary>
    public int Created { get; init; }

    /// <summary>
    /// Number of tickets resolved on this date
    /// </summary>
    public int Resolved { get; init; }

    /// <summary>
    /// Number of tickets closed on this date
    /// </summary>
    public int Closed { get; init; }
}

/// <summary>
/// DTO for priority distribution item (for pie/bar chart)
/// </summary>
public sealed record PriorityDistributionDto
{
    /// <summary>
    /// Priority level (CRITICAL, HIGH, MEDIUM, LOW)
    /// </summary>
    public string Priority { get; init; } = string.Empty;

    /// <summary>
    /// Number of active tickets with this priority
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Percentage of total active tickets
    /// </summary>
    public decimal Percentage { get; init; }
}
